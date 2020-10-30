using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription
{
    /// <summary>
    /// Factory for providing a SubscriberClient. Done for ease of testing.
    /// </summary>
    internal sealed class SubscriberClientFactory : ISubscriberClientFactory
    {
        /// <inheritdoc/>
        public async Task<SubscriberClient> GetSubscriberClient(
            SubscriptionName subscriptionName,
            PubSubOptions options)
        {
            var settings = new SubscriberClient.Settings
            {
                AckDeadline = TimeSpan.FromSeconds(options.AckDeadlineSeconds),
                AckExtensionWindow = TimeSpan.FromSeconds(options.AckExtensionWindowSeconds)
            };
            return await SubscriberClient.CreateAsync(subscriptionName, null, settings);
        }
    }
}