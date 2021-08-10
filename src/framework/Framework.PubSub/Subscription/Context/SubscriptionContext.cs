using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using HumanaEdge.Webcore.Core.PubSub.Subscription;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Context
{
    /// <summary>
    /// Contains the context from the gcp pub sub message.
    /// </summary>
    internal sealed class SubscriptionContext : ISubscriptionContext
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="requestCancelledToken">The cancellation token.</param>
        public SubscriptionContext(string messageId, CancellationToken requestCancelledToken)
        {
            Items = new ConcurrentDictionary<object, object>();
            MessageId = messageId;
            RequestCancelledToken = requestCancelledToken;
        }

        /// <summary>
        ///     Item storage. Used for generic storage of data related to the current context.
        /// </summary>
        public IDictionary<object, object> Items { get; }

        /// <summary>
        /// The original message coming from the GCP Pub Sub.
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// The pub sub cancellation token.
        /// </summary>
        public CancellationToken RequestCancelledToken { get; }
    }
}