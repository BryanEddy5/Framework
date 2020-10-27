using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Telemetry;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription
{
    /// <inheritdoc />
    public sealed class PubSubHostedService<TMessage> : BaseSubscriberHostedService<TMessage>
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="telemetryFactory">The telemetry manager.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="config">The configuration for processing the message.</param>
        /// <param name="subscriberClientFactory">
        /// A factory that generates a
        /// <see cref="T:Google.Cloud.PubSub.V1.SubscriberClient" />.
        /// </param>
        /// <param name="subOrchestrationService">
        /// A service that performs the business logic orchestration on the subscribed
        /// message.
        /// </param>
        public PubSubHostedService(
            ILogger<BaseSubscriberHostedService<TMessage>> logger,
            IOptionsMonitor<PubSubOptions> config,
            ISubscriberClientFactory subscriberClientFactory,
            ISubOrchestrationService<TMessage> subOrchestrationService,
            ITelemetryFactory telemetryFactory)
            : base(logger, config, subscriberClientFactory, subOrchestrationService, telemetryFactory)
        {
        }
    }
}