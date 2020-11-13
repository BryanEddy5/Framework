using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <summary>
    /// A factory for generating a <see cref="PublisherServiceApiClient" />.
    /// </summary>
    internal interface IPublisherClientFactory
    {
        /// <summary>
        /// Creates the <see cref="PublisherServiceApiClient" />.
        /// </summary>
        /// <param name="topicName">The topic name for the message to be published to.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see cref="PublisherServiceApiClient" /> for publishing messages to a topic.</returns>
        Task<IInternalPublisherClient> CreateAsync(TopicName topicName, CancellationToken cancellationToken);
    }
}