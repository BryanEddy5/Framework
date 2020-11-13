using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <summary>
    /// An internal client that publishes a message to a topic.
    /// </summary>
    public interface IInternalPublisherClient
    {
        /// <summary>
        /// Publishes a message to a topic.
        /// </summary>
        /// <param name="publishRequest">The request for the publish method.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The published response.</returns>
        Task<PublishResponse> PublishAsync(PublishRequest publishRequest, CancellationToken cancellationToken);
    }
}