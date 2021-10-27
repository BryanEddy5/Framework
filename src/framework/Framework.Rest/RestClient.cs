using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Core.Rest.Resiliency;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using HumanaEdge.Webcore.Framework.Rest.Resiliency;
using HumanaEdge.Webcore.Framework.Rest.Transformations;
using Polly;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class RestClient : IRestClient
    {
        private readonly RestClientOptions _options;

        private readonly IPollyContextFactory _pollyContextFactory;

        private readonly IRequestTransformationFactory _requestTransformationFactory;

        private readonly IRequestTransformationService _requestTransformationService;

        private readonly ITelemetryFactory _telemetryFactory;

        private readonly string _clientName;

        private readonly IInternalClientFactory _internalClientFactory;

        private readonly IHttpAlertingService _httpAlerting;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="clientName">The name type of the <see cref="System.Net.Http.HttpClient" />.</param>
        /// <param name="internalClientFactory">A factory for generating <see cref="IInternalClient" /> for sending the request.</param>
        /// <param name="options">Configuration settings for outbound requests for the instance of <see cref="IRestClient" />.</param>
        /// <param name="pollyContextFactory">Generates Polly <see cref="Context"/> to be leveraged by consumers.</param>
        /// <param name="requestTransformationFactory">A factory for creating a request transformation service.</param>
        /// <param name="httpAlerting">A service for managing alerts.</param>
        /// <param name="telemetryFactory">A factory associated with telemetry.</param>
        public RestClient(
            string clientName,
            IInternalClientFactory internalClientFactory,
            RestClientOptions options,
            IPollyContextFactory pollyContextFactory,
            IRequestTransformationFactory requestTransformationFactory,
            IHttpAlertingService httpAlerting,
            ITelemetryFactory telemetryFactory = null!)
        {
            _clientName = clientName;
            _internalClientFactory = internalClientFactory;
            _options = options;
            _pollyContextFactory = pollyContextFactory;
            _requestTransformationFactory = requestTransformationFactory;
            _httpAlerting = httpAlerting;
            _telemetryFactory = telemetryFactory;

            _requestTransformationService = _requestTransformationFactory.Create(_options);
        }

        private IInternalClient InternalHttpClient =>
            _internalClientFactory.CreateClient(_clientName, _options.BaseUri, _options.Timeout);

        /// <inheritdoc />
        public async Task<FileResponse> GetFileAsync(RestRequest fileRequest, CancellationToken cancellationToken)
        {
            return await SendInternalAsync(
                fileRequest,
                cancellationToken,
                _requestTransformationService.ConvertToHttpRequestMessage,
                _requestTransformationService.ConvertToStreamResponse);
        }

        /// <inheritdoc />
        public async Task<FileResponse> GetFileAsync<TRequest>(
            RestRequest<TRequest> fileRequest,
            CancellationToken cancellationToken)
        {
            return await SendInternalAsync(
                fileRequest,
                cancellationToken,
                _requestTransformationService.ConvertToHttpRequestMessage,
                _requestTransformationService.ConvertToStreamResponse);
        }

        /// <inheritdoc />
        public async Task<RestResponse> SendAsync(RestRequest restRequest, CancellationToken cancellationToken)
        {
            return await SendInternalAsync(
                restRequest,
                cancellationToken,
                _requestTransformationService.ConvertToHttpRequestMessage,
                _requestTransformationService.ConvertToRestResponse);
        }

        /// <inheritdoc />
        public async Task<RestResponse> SendAsync<TRequest>(RestRequest<TRequest> restRequest, CancellationToken cancellationToken)
        {
            return await SendInternalAsync(
                restRequest,
                cancellationToken,
                _requestTransformationService.ConvertToHttpRequestMessage,
                _requestTransformationService.ConvertToRestResponse);
        }

        /// <summary>
        /// Orchestrates the actual HTTP request. Applies any transformations, converted to and from HTTP- classes.
        /// </summary>
        /// <typeparam name="TRestRequest">The type of the rest request.</typeparam>
        /// <typeparam name="TRestResponse">The type of the rest response.</typeparam>
        /// <param name="restRequest">The rest request object.</param>
        /// <param name="cancellationToken">The cancellation token for the request.</param>
        /// <param name="restRequestConverter">Applies middleware transformation of the request.</param>
        /// <param name="restResponseConverter">Applies middleware transformation of the response.</param>
        private async Task<TRestResponse> SendInternalAsync<TRestRequest, TRestResponse>(
            TRestRequest restRequest,
            CancellationToken cancellationToken,
            Func<TRestRequest, HttpRequestMessage> restRequestConverter,
            Func<HttpResponseMessage, Task<TRestResponse>> restResponseConverter)
            where TRestRequest : RestRequest
            where TRestResponse : BaseRestResponse
        {
            var transformedRestRequest = await _requestTransformationService.TransformRequest(restRequest, cancellationToken);
            var contextData = _pollyContextFactory.Create()
                .WithRestRequest(transformedRestRequest);

            async Task<BaseRestResponse> Action(Context context, CancellationToken token) =>
                await SendInternalAsync(token, restRequestConverter, restResponseConverter, transformedRestRequest);

            if (_options.ResiliencePolicies.Length > 1)
            {
                var poly = Policy.WrapAsync(_options.ResiliencePolicies);
                return (TRestResponse)await poly.ExecuteAsync(Action, contextData, cancellationToken);
            }
            else
            {
                return (TRestResponse)await _options.ResiliencePolicies.First()
                    .ExecuteAsync(Action, contextData, cancellationToken);
            }
        }

        /// <summary>
        /// The encapsulation of the action of sending the http request message.
        /// This is an action that represents the act of sending the message and getting a response.
        /// If this should fail, the resilience will be engaged in the other SendInternalAsync method.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the request.</param>
        /// <param name="restRequestConverter">Applies middleware transformation of the request.</param>
        /// <param name="restResponseConverter">Applies middleware transformation of the response.</param>
        /// <param name="restRequest">The rest request object.</param>
        /// <typeparam name="TRestRequest">The type of the rest request.</typeparam>
        /// <typeparam name="TRestResponse">The type of the rest response.</typeparam>
        /// <returns>The received response from the endpoint.</returns>
        private async Task<BaseRestResponse> SendInternalAsync<TRestRequest, TRestResponse>(
            CancellationToken cancellationToken,
            Func<TRestRequest, HttpRequestMessage> restRequestConverter,
            Func<HttpResponseMessage, Task<TRestResponse>> restResponseConverter,
            TRestRequest restRequest)
            where TRestRequest : RestRequest
            where TRestResponse : BaseRestResponse
        {
            var httpRequestMessage = restRequestConverter(restRequest);
            HttpResponseMessage httpResponse = null!;
            TRestResponse convertedResponse;
            var isAlert = true;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var startTime = DateTimeOffset.UtcNow;
            try
            {
                httpResponse = await InternalHttpClient.SendAsync(httpRequestMessage, cancellationToken);
                convertedResponse = await restResponseConverter(httpResponse);
                isAlert = _httpAlerting.IsHttpAlert(
                    convertedResponse,
                    restRequest.AlertCondition,
                    _options.AlertCondition);
            }
            finally
            {
                stopWatch.Stop();
                TrackTelemetry(
                    httpRequestMessage,
                    httpResponse,
                    startTime,
                    stopWatch.ElapsedMilliseconds,
                    isAlert);
            }

            if (isAlert)
            {
                _httpAlerting.ThrowIfAlertedAndNeedingException(
                    restRequest.AlertCondition,
                    _options.AlertCondition);
            }

            return convertedResponse;
        }

        /// <summary>
        /// Short-hand method for tracking telemetry in this REST Client.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequestMessage" /> used for this telemetry.</param>
        /// <param name="response">The <see cref="HttpResponseMessage" /> used for this telemetry.</param>
        /// <param name="startTime">The start time of the dependency.</param>
        /// <param name="duration">The total duration of this request.</param>
        /// <param name="isAlert">Whether or not the telemetry contains an alert.</param>
        private void TrackTelemetry(
            HttpRequestMessage request,
            HttpResponseMessage? response,
            DateTimeOffset startTime,
            double duration,
            bool isAlert)
        {
            _telemetryFactory.TrackDependencyHttpTelemetry(
                startTime,
                duration,
                ((int?)response?.StatusCode)?.ToString()!,
                request.Method.ToString(),
                request.RequestUri?.ToString()!,
                isAlert,
                success: response != null && response.IsSuccessStatusCode);
        }
    }
}