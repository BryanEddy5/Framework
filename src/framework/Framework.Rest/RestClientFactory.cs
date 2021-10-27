using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.Rest.Resiliency;
using HumanaEdge.Webcore.Framework.Rest.Transformations;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <inheritdoc />
    internal sealed class RestClientFactory : IRestClientFactory
    {
        private readonly IInternalClientFactory _internalClientFactory;

        private readonly IPollyContextFactory _pollyContextFactory;

        private readonly IRequestTransformationFactory _requestTransformationFactory;

        private readonly ITelemetryFactory _telemetryFactory;

        private readonly IHttpAlertingService _httpAlerting;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientFactory" /> class.
        /// </summary>
        /// <param name="internalClientFactory">A factory for generating a rest client.</param>
        /// <param name="pollyContextFactory">A factory for creating Polly Context.</param>
        /// <param name="requestTransformationFactory">A factory for creating a request transformation service.</param>
        /// <param name="httpAlerting">The alerting service for http telemetry.</param>
        /// <param name="telemetryFactory">A factory associated with telemetry.</param>
        public RestClientFactory(
            IInternalClientFactory internalClientFactory,
            IPollyContextFactory pollyContextFactory,
            IRequestTransformationFactory requestTransformationFactory,
            IHttpAlertingService httpAlerting,
            ITelemetryFactory telemetryFactory = null!)
        {
            _internalClientFactory = internalClientFactory;
            _pollyContextFactory = pollyContextFactory;
            _requestTransformationFactory = requestTransformationFactory;
            _httpAlerting = httpAlerting;
            _telemetryFactory = telemetryFactory;
        }

        /// <inheritdoc />
        public IRestClient CreateClient<TRestClient>(RestClientOptions options)
        {
            return new RestClient(
                typeof(TRestClient).Name,
                _internalClientFactory,
                options,
                _pollyContextFactory,
                _requestTransformationFactory,
                _httpAlerting,
                _telemetryFactory);
        }
    }
}