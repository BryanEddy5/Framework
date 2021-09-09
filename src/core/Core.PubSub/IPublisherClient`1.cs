using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.PubSub.Alerting;

namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// A service for publishing messages to a topic.
    /// </summary>
    /// <typeparam name="TMessage">The published message shape.</typeparam>
    public interface IPublisherClient<in TMessage>
    {
        /// <summary>
        /// The <see cref="AlertCondition"/> for this client.
        /// </summary>
        AlertCondition ClientAlertCondition { get; }

        /// <summary>
        /// Publishes multiple messages to a topic.
        /// </summary>
        /// <param name="message">An array of messages to be published.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The message id's for the published messages.</returns>
        Task<IReadOnlyList<string>> PublishAsync(TMessage[] message, CancellationToken cancellationToken);

        /// <summary>
        /// Publishes a message to a topic.
        /// </summary>
        /// <param name="message">The message to be published.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The message id for the published message.</returns>
        Task<string> PublishAsync(TMessage message, CancellationToken cancellationToken);
    }
}