using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Framework.PubSub.Subscription;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs
{
    /// <inheritdoc />
    public sealed class PubSubHostedService : BaseSubscriberHostedService<Foo>
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
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
            ILogger<BaseSubscriberHostedService<Foo>> logger,
            IOptionsMonitor<PubSubOptions> config,
            ISubscriberClientFactory subscriberClientFactory,
            ISubOrchestrationService<Foo> subOrchestrationService)
            : base(logger, config, subscriberClientFactory, subOrchestrationService)
        {
        }
    }
}