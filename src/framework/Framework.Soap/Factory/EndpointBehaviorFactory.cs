using System.ServiceModel.Description;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Client.Factory;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.Soap.Contracts;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;

namespace HumanaEdge.Webcore.Framework.Soap.Factory
{
    /// <inheritdoc />
    internal sealed class EndpointBehaviorFactory : IEndpointBehaviorFactory
    {
        /// <inheritdoc cref="ITelemetryFactory"/>
        private readonly ITelemetryFactory _telemetryFactory;

        /// <inheritdoc cref="IPollyContextFactory"/>
        private readonly IPollyContextFactory _pollyContextFactory;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="telemetryFactory">The factory for generating telemetry.</param>
        /// <param name="pollyContextFactory">The factory for generating polly-contexts.</param>
        public EndpointBehaviorFactory(ITelemetryFactory telemetryFactory, IPollyContextFactory pollyContextFactory)
        {
            _telemetryFactory = telemetryFactory;
            _pollyContextFactory = pollyContextFactory;
        }

        /// <inheritdoc />
        public IEndpointBehavior Create<TClient>(SoapClientOptions soapClientOptions)
        {
            return new EndpointBehavior(
                typeof(TClient).FullName!,
                soapClientOptions,
                _telemetryFactory,
                _pollyContextFactory);
        }
    }
}