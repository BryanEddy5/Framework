using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common.Extensions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using HumanaEdge.Webcore.Framework.Soap.Extensions;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;
using Polly;

namespace HumanaEdge.Webcore.Framework.Soap
{
    /// <summary>
    /// The handler for managing the additional functionality when leveraging a SOAPey client.<br/>
    /// Used to implement things like our telemetry and the various <see cref="SoapClientOptions"/> functionalities
    /// into our outbound SOAP http requests.
    /// </summary>
    /// <remarks>
    /// This message handling delegate is SOAP's equivalent "RestClient" implementation.
    /// </remarks>
    internal sealed class SoapHttpMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// The client name.
        /// </summary>
        private readonly string _clientName;

        /// <summary>
        /// The client configuration.
        /// </summary>
        private readonly SoapClientOptions _soapClientOptions;

        /// <inheritdoc cref="ITelemetryFactory"/>
        private readonly ITelemetryFactory _telemetryFactory;

        /// <inheritdoc cref="IPollyContextFactory"/>
        private readonly IPollyContextFactory _pollyContextFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="clientName">The client name.</param>
        /// <param name="httpMessageHandler">The handler to pass upwards.</param>
        /// <param name="soapClientOptions">The client configuration.</param>
        /// <param name="telemetryFactory">The factory for generating telemetry.</param>
        /// <param name="pollyContextFactory">The factory for generating polly-context.</param>
        public SoapHttpMessageHandler(
            string clientName,
            HttpMessageHandler httpMessageHandler,
            SoapClientOptions soapClientOptions,
            ITelemetryFactory telemetryFactory,
            IPollyContextFactory pollyContextFactory)
                : base(httpMessageHandler)
        {
            _clientName = clientName;
            _soapClientOptions = soapClientOptions;
            _telemetryFactory = telemetryFactory;
            _pollyContextFactory = pollyContextFactory;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Here is where we oversee the whole orchestration with resilience.
        /// </remarks>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var resilience = _soapClientOptions.ResiliencePolicies;
            if (!resilience.Any())
            {
                return await SendSoapRequestAsync(request, cancellationToken);
            }

            async Task<HttpResponseMessage> SendFunc(Context context, CancellationToken token)
            {
                return await SendSoapRequestAsync(request, token);
            }

            var pollyContext = _pollyContextFactory.Create();
            if (resilience.Length == 1)
            {
                return await resilience.First()
                    .ExecuteAsync(SendFunc, pollyContext, cancellationToken);
            }

            return await Policy
                .WrapAsync(resilience)
                .ExecuteAsync(SendFunc, pollyContext, cancellationToken);
        }

        /// <summary>
        /// The actual sending associated with the SOAP request.<br/>
        /// This is literally where the magic happens.
        /// </summary>
        /// <param name="request">The request message to send.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="HttpResponseMessage"/>.</returns>
        private async Task<HttpResponseMessage> SendSoapRequestAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponse = null!;
            var stopWatch = new Stopwatch();
            var transformedRequest = await TransformRequest(request, cancellationToken);

            try
            {
                httpResponse = await base.SendAsync(transformedRequest, cancellationToken);
            }
            finally
            {
                stopWatch.Stop();
                TrackTelemetry(transformedRequest, httpResponse, DateTimeOffset.UtcNow, stopWatch.ElapsedMilliseconds);
                stopWatch.Reset();
            }

            return httpResponse;
        }

        /// <summary>
        /// Modifies the SOAPey http request before it is sent out.
        /// </summary>
        /// <param name="httpRequestMessage">The incoming request to transform.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The transformed request.</returns>
        private async Task<HttpRequestMessage> TransformRequest(
            HttpRequestMessage httpRequestMessage,
            CancellationToken cancellationToken)
        {
            var transformedRequest = httpRequestMessage;
            var defaultHeaderKeysToAdd = _soapClientOptions
                .Headers.Keys
                .Except(httpRequestMessage.Headers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value).Keys);

            defaultHeaderKeysToAdd.ForEach(
                key =>
                {
                    transformedRequest.Headers.SetHeader(key, _soapClientOptions.Headers[key]);
                });

            // TODO: apply sync/async transformations here.
            await Task.CompletedTask;

            return transformedRequest;
        }

        /// <summary>
        /// Short-hand for tracking telemetry.
        /// </summary>
        /// <param name="request">The http request message.</param>
        /// <param name="response">The http response message- which may be null if the request failed.</param>
        /// <param name="startTime">When the request was sent.</param>
        /// <param name="duration">How long it took to receive a response from the server, positive or negative.</param>
        private void TrackTelemetry(
            HttpRequestMessage request,
            HttpResponseMessage? response,
            DateTimeOffset startTime,
            double duration)
        {
            // TODO: use the client name in the below described telemetry!
            var soapClientNameForTelemetry = _clientName;

            // TODO: switch this to a custom tracking for SOAP requests and add the fact that it is SOAP into that.
            _telemetryFactory.TrackDependencyHttpTelemetry(
                startTime: startTime,
                duration: duration,
                responseCode: ((int?)response?.StatusCode)?.ToString()!,
                httpMethod: request.Method.ToString()!,
                uri: request.RequestUri?.ToString()!,
                alert: !(response != null || !response!.IsSuccessStatusCode),
                success: response != null || !response!.IsSuccessStatusCode);
        }
    }
}