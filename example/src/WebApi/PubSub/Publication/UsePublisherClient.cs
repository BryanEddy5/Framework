using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Publication
{
    /// <summary>
    /// Illustrates how to utilize the <see cref="IPublisherClient{TMessage}"/>.
    /// </summary>
    public class UsePublisherClient
    {
        private readonly IPublisherClient<FooContract> _publisherClient;

        /// <summary>
        /// Inject <see cref="IPublisherClient{TMessage}"/> to be leveraged.
        /// </summary>
        /// <param name="publisherClient">The client for publishing a message to a topic.</param>
        public UsePublisherClient(IPublisherClient<FooContract> publisherClient)
        {
            _publisherClient = publisherClient;
        }

        /// <summary>
        /// An example of invoking the publisher client with a single message to be published.
        /// </summary>
        /// <param name="fooContract">The message to be published.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The message id for the published message.</returns>
        public async Task<string> ExecuteAsync(FooContract fooContract, CancellationToken cancellationToken)
        {
           return await _publisherClient.PublishAsync(fooContract, cancellationToken);
        }

        /// <summary>
        /// An example of invoking the publisher client with an array of messages to be published.
        /// </summary>
        /// <param name="fooContracts">A collection of messages to be published.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of published messages for the messages that were published.</returns>
        public async Task<IReadOnlyList<string>> ExecuteArrayAsync(FooContract[] fooContracts, CancellationToken cancellationToken)
        {
            return await _publisherClient.PublishAsync(fooContracts, cancellationToken);
        }
    }
}