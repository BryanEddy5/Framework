using System.Collections.Generic;
using System.Threading;

namespace HumanaEdge.Webcore.Core.PubSub.Subscription
{
    /// <summary>
    /// Subscription message context.
    /// </summary>
    public interface ISubscriptionContext
    {
        /// <summary>
        ///     Item storage. Used for generic storage of data related to the current context.
        /// </summary>
        IDictionary<object, object> Items { get; }

        /// <summary>
        /// The unique message id.
        /// </summary>
        string MessageId { get; }

        /// <summary>
        /// The pub sub cancellation token.
        /// </summary>
        CancellationToken RequestCancelledToken { get; }
    }
}