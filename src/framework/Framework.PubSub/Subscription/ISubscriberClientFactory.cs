using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub
{
    /// <summary>
    /// A Factory for building Pubsub Subscriber Clients.
    /// </summary>
    public interface ISubscriberClientFactory
    {
        /// <summary>
        /// Get the desired <see cref="SubscriberClient" />.
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription to connect to.</param>
        /// <returns>The subscriber client.</returns>
        Task<SubscriberClient> GetSubscriberClient(SubscriptionName subscriptionName);
    }
}