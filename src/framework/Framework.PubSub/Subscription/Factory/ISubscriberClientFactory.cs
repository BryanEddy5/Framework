using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Factory
{
    /// <summary>
    /// A Factory for building Pubsub Subscriber Clients.
    /// </summary>
    internal interface ISubscriberClientFactory
    {
        /// <summary>
        /// Get the desired <see cref="SubscriberClient" />.
        /// </summary>
        /// <param name="pubSubName">The named type of the pubsub subscriber..</param>
        /// <returns>The subscriber client.</returns>
        Task<SubscriberClient> Create(string pubSubName);
    }
}