using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Framework.PubSub.TraceContext;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription
{
    /// <inheritdoc />
    public sealed class PubSubHostedService<TMessage> : BaseSubscriberHostedService<TMessage>
        where TMessage : class
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
        /// <param name="activityFactory">A factory for creating a new activity with W3C trace context.</param>
        public PubSubHostedService(
            ILoggerFactory logger,
            IOptionsMonitor<PubSubOptions> config,
            ISubscriberClientFactory subscriberClientFactory,
            ISubOrchestrationService<TMessage> subOrchestrationService,
            ITelemetryFactory telemetryFactory,
            IActivityFactory activityFactory)
            : base(
                logger.CreateLogger<BaseSubscriberHostedService<TMessage>>(),
                config,
                subscriberClientFactory,
                subOrchestrationService,
                telemetryFactory,
                activityFactory)
        {
        }
    }
}