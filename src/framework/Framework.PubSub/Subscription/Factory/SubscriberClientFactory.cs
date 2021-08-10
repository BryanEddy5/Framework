using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Factory
{
    /// <summary>
    /// Factory for providing a SubscriberClient. Done for ease of testing.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class SubscriberClientFactory : ISubscriberClientFactory
    {
        private readonly IOptionsMonitor<PubSubOptions> _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriberClientFactory"/> class.
        /// </summary>
        /// <param name="options">The PubSub configuration options.</param>
        public SubscriberClientFactory(IOptionsMonitor<PubSubOptions> options)
        {
            _options = options;
        }

        /// <inheritdoc/>
        public async Task<SubscriberClient> Create(string pubSubName)
        {
            var options = _options.Get(pubSubName);
            var subscriptionName = new SubscriptionName(options.ProjectId, options.Name);

            var settings = new SubscriberClient.Settings
            {
                AckDeadline = TimeSpan.FromSeconds(options.AckDeadlineSeconds),
                AckExtensionWindow = TimeSpan.FromSeconds(options.AckExtensionWindowSeconds),
                FlowControlSettings = new FlowControlSettings(
                    options.MaxMessageCount,
                    options.MaxMessageByteCount)
            };
            return await SubscriberClient.CreateAsync(subscriptionName, null, settings);
        }
    }
}