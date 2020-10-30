using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;

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
        /// <param name="options">Configuration settings for the subscription client.</param>
        /// <returns>The subscriber client.</returns>
        Task<SubscriberClient> GetSubscriberClient(SubscriptionName subscriptionName, PubSubOptions options);
    }
}