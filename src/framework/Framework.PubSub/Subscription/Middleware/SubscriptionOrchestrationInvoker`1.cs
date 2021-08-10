using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Converters;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware
{
    /// <summary>
    /// Invokes the underlying service to handle the message.
    /// </summary>
    /// <typeparam name="TMessage">The message shape.</typeparam>
    public class SubscriptionOrchestrationInvoker<TMessage> : ISubscriptionMiddleware<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="serviceProvider">The service provider. </param>
        public SubscriptionOrchestrationInvoker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task NextAsync(
            ISubscriptionContext subscriptionMessage,
            MessageDelegate messageDelegate)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var subOrchestrationService = scope.ServiceProvider.GetRequiredService<ISubOrchestrationService<TMessage>>();
                var pubsubMessage = (PubsubMessage)subscriptionMessage.Items[ContextKeys.SubscriptionContextKey];

                await subOrchestrationService.ExecuteAsync(
                    pubsubMessage.ToSubscriptionMessage<TMessage>(),
                    subscriptionMessage.RequestCancelledToken);
            }
        }
    }
}