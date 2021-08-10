using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.PubSub.Subscription
{
    /// <summary>
    /// A delegate for invoking the middleware in the subscription pipeline.
    /// </summary>
    /// <param name="context">The message being passed.</param>
    /// <returns>An awaitable task.</returns>
    public delegate Task MessageDelegate(ISubscriptionContext context);

    /// <summary>
    /// Pipeline middleware for subscriptions.
    /// </summary>
    /// <typeparam name="TMessage">The message shape.</typeparam>
    public interface ISubscriptionMiddleware<TMessage>
    {
        /// <summary>
        /// Calls the next middleware in the pipeline.
        /// </summary>
        /// <param name="subscriptionMessage">The subscription data.</param>
        /// <param name="messageDelegate">The delegate to invoke the next middleware in the pipeline. </param>
        /// <returns>An ack or nack of the message.</returns>
        Task NextAsync(ISubscriptionContext subscriptionMessage, MessageDelegate messageDelegate);
    }
}