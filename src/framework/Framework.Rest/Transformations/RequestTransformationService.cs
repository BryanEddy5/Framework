using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace HumanaEdge.Webcore.Framework.Rest.Transformations
{
    /// <inheritdoc cref="IRequestTransformationService"/>
    internal sealed class RequestTransformationService : IRequestTransformationService
    {
        private const string CloudTraceHeader = "x-cloud-trace-context";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAccessTokenCacheService _accessTokenCacheService;

        private readonly IDictionary<MediaType, IMediaTypeFormatter> _mediaTypeFormatters;

        private readonly RestClientOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTransformationService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Used to access the http context.</param>
        /// <param name="accessTokenCacheService">A cache for tokens.</param>
        /// <param name="mediaTypeFormatters">A collection of media type formatters.</param>
        /// <param name="options">REST client options.</param>
        public RequestTransformationService(
            IAccessTokenCacheService accessTokenCacheService,
            IHttpContextAccessor httpContextAccessor,
            IEnumerable<IMediaTypeFormatter> mediaTypeFormatters,
            RestClientOptions options)
        {
            _httpContextAccessor = httpContextAccessor;
            _accessTokenCacheService = accessTokenCacheService;
            _mediaTypeFormatters = mediaTypeFormatters.SelectMany(
                    formatter => formatter.MediaTypes.Select(
                        mediaType =>
                            new KeyValuePair<MediaType, IMediaTypeFormatter>(mediaType, formatter)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            _options = options;
        }

        /// <inheritdoc/>
        public async Task<TRestRequest> TransformRequest<TRestRequest>(
            TRestRequest restRequest,
            CancellationToken cancellationToken)
            where TRestRequest : RestRequest
        {
            var transformedRequest = restRequest;

            // greedy allocation to array to avoid exceptions due to modifying the collection while enumerating over it.
            var defaultHeaderKeysToAdd = _options.DefaultHeaders.Keys.Except(restRequest.Headers.Keys);

            // add the client-level defaults.
            foreach (var key in defaultHeaderKeysToAdd)
            {
                transformedRequest.Headers[key] = _options.DefaultHeaders[key];
            }

            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue(CloudTraceHeader, out var value))
            {
                transformedRequest.Headers[CloudTraceHeader] = value;
            }

            // apply synchronous transformations.
            foreach (var transformation in _options.RestRequestMiddleware)
            {
                transformedRequest = (TRestRequest)transformation(restRequest);
            }

            foreach (var asyncTransformation in _options.RestRequestMiddlewareAsync)
            {
                transformedRequest = (TRestRequest)await asyncTransformation(restRequest, cancellationToken);
            }

            if (_options.BearerTokenFactory.HasValue)
            {
                var (factory, token) = _options.BearerTokenFactory.Value;
                var bearerToken = await _accessTokenCacheService.GetAsync(factory, token, cancellationToken);
                transformedRequest.Headers[HeaderNames.Authorization] = $"Bearer {bearerToken}";
            }

            return transformedRequest;
        }

        /// <inheritdoc/>
        public HttpRequestMessage ConvertToHttpRequestMessage<TRequest>(RestRequest<TRequest> request)
        {
            var httpRequestMessage = ConvertToHttpRequestMessage(request as RestRequest);
            try
            {
                if (!_mediaTypeFormatters.TryGetValue(request.MediaType, out var mediaTypeFormatter))
                {
                    throw new FormatFailedRestException(
                        $"A formatter could not be located for media type {nameof(request.MediaType)}");
                }

                if (!mediaTypeFormatter.TryFormat(
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

        /// <inheritdoc/>
        public HttpRequestMessage ConvertToHttpRequestMessage(RestRequest request)
        {
            var message = new HttpRequestMessage(request.HttpMethod, request.RelativePath);
            foreach (var header in request.Headers.Keys)
            {
                message.Headers.Add(header, (IEnumerable<string>)request.Headers[header]);
            }

            return message;
        }

        /// <inheritdoc/>
        public async Task<RestResponse> ConvertToRestResponse(HttpResponseMessage httpResponseMessage)
        {
            var responseBytes = httpResponseMessage.Content != null
                ? await httpResponseMessage.Content.ReadAsByteArrayAsync()
                : Array.Empty<byte>();

            var contentType = httpResponseMessage.Content?.Headers.ContentType;
            var deserializer = new RestResponseDeserializer(_mediaTypeFormatters, contentType, responseBytes, _options);
            return new RestResponse(
                httpResponseMessage.IsSuccessStatusCode,
                deserializer,
                httpResponseMessage.StatusCode,
                httpResponseMessage.Headers.Location);
        }

        /// <inheritdoc/>
        public async Task<FileResponse> ConvertToStreamResponse(HttpResponseMessage httpResponseMessage)
        {
            var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            return new FileResponse(
                httpResponseMessage.IsSuccessStatusCode,
                responseStream,
                httpResponseMessage.StatusCode,
                httpResponseMessage.Content.Headers.ContentType?.MediaType,
                httpResponseMessage.Content.Headers.ContentDisposition?.FileName);
        }
    }
}