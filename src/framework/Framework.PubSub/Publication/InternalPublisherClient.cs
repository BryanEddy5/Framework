using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public class InternalPublisherClient : IInternalPublisherClient
    {
        private readonly PublisherServiceApiClient _client;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="client">The GCP implementation for publishing the message to a topic.</param>
        public InternalPublisherClient(PublisherServiceApiClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public async Task<PublishResponse> PublishAsync(
            PublishRequest publishRequest,
            CancellationToken cancellationToken) =>
            await _client.PublishAsync(publishRequest, cancellationToken);
    }
}