using System;
using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.PubSub.Subscription;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware.Builder
{
    /// <inheritdoc />
    internal class MiddlewareBuilder<TMessage>
        : IMiddlewareBuilder<TMessage>
    {
        private readonly IList<Func<MessageDelegate, MessageDelegate>> _container =
            new List<Func<MessageDelegate, MessageDelegate>>();

        private MessageDelegate? _messageDelegate;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="middlewares">The middlewares that have been registered.</param>
        public MiddlewareBuilder(IEnumerable<ISubscriptionMiddleware<TMessage>> middlewares)
        {
            foreach (var middleware in middlewares)
            {
                _container.Add(
                    subscriptionDelegate =>
                    {
                        return message =>
                        {
                            return middleware.NextAsync(message, subscriptionDelegate);
                        };
                    });
            }
        }

        /// <inheritdoc />
        public MessageDelegate Build()
        {
            return _messageDelegate ??= _container.Reverse()
                .Aggregate<Func<MessageDelegate, MessageDelegate>,
                    MessageDelegate>(
                    null !,
                    (subscriptionDelegate, delegateFactory) => delegateFactory(subscriptionDelegate));
        }
    }
}