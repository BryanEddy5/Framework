using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Web;
using Polly;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class RestClient : IRestClient
    {
        private readonly IInternalClient _httpClient;

        private readonly IMediaTypeFormatter[] _mediaTypeFormatters;

        private readonly RestClientOptions _options;

        private readonly IRequestIdAccessor? _requestIdAccessor;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="clientName">The name type of the <see cref="HttpClient"/>.</param>
        /// <param name="internalClientFactory">A factory for generating <see cref="IInternalClient"/> for sending the request.</param>
        /// <param name="requestIdAccessor">Provides access to the request id.</param>
        /// <param name="options">Configuration settings for outbound requests for the instance of <see cref="IRestClient"/>.</param>
        /// <param name="mediaTypeFormatters">A collection of media type formatters.</param>
        public RestClient(
            string clientName,
            IInternalClientFactory internalClientFactory,
            IRequestIdAccessor? requestIdAccessor,
            RestClientOptions options,
            IMediaTypeFormatter[] mediaTypeFormatters)
        {
            _httpClient = internalClientFactory.CreateClient(clientName, options.BaseUri, options.Timeout);
            _requestIdAccessor = requestIdAccessor;
            _options = options;
            _mediaTypeFormatters = mediaTypeFormatters;
        }

        /// <inheritdoc />
        public async Task<RestResponse> SendAsync(RestRequest restRequest, CancellationToken cancellationToken)
        {
            return await SendInternalAsync(restRequest, cancellationToken, ConvertToHttpRequestMessage);
        }

        /// <inheritdoc />
        public async Task<RestResponse> SendAsync<TRequest>(
            RestRequest<TRequest> restRequest,
            CancellationToken cancellationToken)
        {
            return await SendInternalAsync(restRequest, cancellationToken, ConvertToHttpRequestMessage);
        }

        /// <summary>
        /// Converts a <see cref="RestRequest{TRequest}" /> to <see cref="HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The rest request to be converted.</param>
        /// <typeparam name="TRequest">The <see cref="Type" /> of the request body.</typeparam>
        /// <returns>A http request to be sent.</returns>
        /// <exception cref="FormatFailedRestException">An exception thrown if the request body could not be serialized.</exception>
        private HttpRequestMessage ConvertToHttpRequestMessage<TRequest>(RestRequest<TRequest> request)
        {
            var httpRequestMessage = ConvertToHttpRequestMessage(request as RestRequest);
            try
            {
                var mediaType =
                    _mediaTypeFormatters.FirstOrDefault(r => r.MediaType.MimeType == request.MediaType.MimeType);
                if (mediaType == null)
                {
                    throw new FormatFailedRestException(
                        $"A formatter could not be located for media type {nameof(request.MediaType)}");
                }

                if (!mediaType.TryFormat(
                    request.MediaType,
                    _options,
                    request.RequestBody,
                    out var httpContent))
                {
                    throw new FormatFailedRestException(
                        $"Could not format media type {nameof(request.MediaType)} for request {typeof(TRequest).Name}");
                }

                httpRequestMessage.Content = httpContent;

                return httpRequestMessage;
            }
            catch (Exception formatException)
            {
                throw new FormatFailedRestException(
                    $"Attempting to format {typeof(TRequest).Name} as media type {request.MediaType.Name} resulted in an exception",
                    formatException);
            }
        }

        /// <summary>
        /// Converts a <see cref="RestRequest" /> to a <see cref="HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The request to convert.</param>
        /// <returns>An <see cref="HttpRequestMessage" />.</returns>
        private HttpRequestMessage ConvertToHttpRequestMessage(RestRequest request)
        {
            var message = new HttpRequestMessage(request.HttpMethod, request.RelativePath);
            foreach (var header in request.Headers.Keys)
            {
                message.Headers.Add(header, (IEnumerable<string>)request.Headers[header]);
            }

            // add correlation id to all outgoing requests.
            if (_requestIdAccessor?.Header != null && _requestIdAccessor?.CorrelationId != null)
            {
                message.Headers.Add(_requestIdAccessor?.Header, _requestIdAccessor?.CorrelationId);
            }

            return message;
        }

        private async Task<RestResponse> ConvertToRestResponse(HttpResponseMessage httpResponseMessage)
        {
            var responseBytes = httpResponseMessage.Content != null
                ? await httpResponseMessage.Content.ReadAsByteArrayAsync()
                : Array.Empty<byte>();

            var contentType = httpResponseMessage.Content?.Headers?.ContentType;
            var deserializer = new RestResponseDeserializer(_mediaTypeFormatters, contentType, responseBytes, _options);
            return new RestResponse(
                httpResponseMessage.IsSuccessStatusCode,
                deserializer,
                httpResponseMessage.StatusCode);
        }

        /// <summary>
        /// Orchestrates the actual HTTP request. Applies any transformations, converted to and from HTTP- classes.
        /// </summary>
        /// <typeparam name="TRestRequest">The type of the rest request.</typeparam>
        /// <param name="restRequest">The rest request object.</param>
        /// A function which converts the <typeparamref name="TRestRequest" />
        /// into an <see cref="HttpRequestMessage" /> .
        /// <param name="cancellationToken">The cancellation token for the request.</param>
        /// <param name="restRequestConverter">Applies middleware transformation of the request.</param>
        private async Task<RestResponse> SendInternalAsync<TRestRequest>(
            TRestRequest restRequest,
            CancellationToken cancellationToken,
            Func<TRestRequest, HttpRequestMessage> restRequestConverter)
            where TRestRequest : RestRequest
        {
            var transformedRestRequest = TransformRequest(restRequest, out var resiliencePolicy);
            return await resiliencePolicy.ExecuteAsync(
                async ct =>
                {
                    var httpRequestMessage = restRequestConverter(transformedRestRequest);
                    var httpResponse = await _httpClient.SendAsync(httpRequestMessage, ct);
                    return await ConvertToRestResponse(httpResponse);
                },
                cancellationToken);
        }

        private TRestRequest TransformRequest<TRestRequest>(
            TRestRequest restRequest,
            out IAsyncPolicy<RestResponse> resiliencePolicy)
            where TRestRequest : RestRequest
        {
            var transformedRequest = restRequest;
            resiliencePolicy = _options.ResiliencePolicy;

            // greedy allocation to array to avoid exceptions due to modifying the collection while enumerating over it.
            var defaultHeaderKeysToAdd = _options.DefaultHeaders.Keys.Except(restRequest.Headers.Keys).ToArray();

            // add the client-level defaults.
            foreach (var key in defaultHeaderKeysToAdd)
            {
                transformedRequest.Headers[key] = _options.DefaultHeaders[key];
            }

            // apply transformations.
            foreach (var transformation in _options.RestRequestMiddleware)
            {
                transformedRequest = (TRestRequest)transformation(restRequest);
            }

            return transformedRequest;
        }
    }
}