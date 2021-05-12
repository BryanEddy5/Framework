using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// A service that acts upon the subscription message received.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to be received from the subscription.</typeparam>
    public interface ISubOrchestrationService<TMessage>
    {
        /// <summary>
        /// Execute the orchestration service.
        /// </summary>
        /// <param name="message">The subscribed message.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(TMessage message, CancellationToken cancellationToken) => throw new NotImplementedException();

        /// <summary>
        /// Execute the orchestration service.
        /// </summary>
        /// <param name="message">The subscribed message.</param>
        /// <param name="cancellationToken">The cancellation Token.</param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(SubscriptionMessage<TMessage> message, CancellationToken cancellationToken) =>
            ExecuteAsync(message.Message.Value, cancellationToken);
    }
}