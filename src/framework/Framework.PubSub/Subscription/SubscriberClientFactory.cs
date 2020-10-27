using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription
{
    /// <summary>
    /// Factory for providing a SubscriberClient. Done for ease of testing.
    /// </summary>
    internal sealed class SubscriberClientFactory : ISubscriberClientFactory
    {
        /// <summary>
        /// Get the desired <see cref="Subscriber.SubscriberClient" />.
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription to connect to.</param>
        /// <returns>The subscriber client.</returns>
        public async Task<SubscriberClient> GetSubscriberClient(SubscriptionName subscriptionName)
        {
            return await SubscriberClient.CreateAsync(subscriptionName);
        }
    }
}