using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Converters;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
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

        private readonly IValidator<TMessage>? _validator;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="serviceProvider">The service provider. </param>
        /// <param name="validator">Validates the model state. </param>
        public SubscriptionOrchestrationInvoker(
            IServiceProvider serviceProvider,
            IValidator<TMessage> validator = null!)
        {
            _serviceProvider = serviceProvider;
            _validator = validator;
        }

        /// <inheritdoc />
        public async Task NextAsync(
            ISubscriptionContext subscriptionMessage,
            MessageDelegate messageDelegate)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var subOrchestrationService =
                    scope.ServiceProvider.GetRequiredService<ISubOrchestrationService<TMessage>>();
                var pubsubMessage = (PubsubMessage)subscriptionMessage.Items[ContextKeys.SubscriptionContextKey];
                var message = pubsubMessage.ToSubscriptionMessage<TMessage>();

                await ValidateModelState(message);

                await subOrchestrationService.ExecuteAsync(
                    pubsubMessage.ToSubscriptionMessage<TMessage>(),
                    subscriptionMessage.RequestCancelledToken);
            }
        }

        private async Task ValidateModelState(SubscriptionMessage<TMessage> subscriptionMessage)
        {
            if (_validator == null)
            {
                return;
            }

            var result = await _validator.ValidateAsync(subscriptionMessage.Message.Value);
            if (!result.IsValid)
            {
                var errors = result.Errors;
                throw new InvalidModelStateException(errors);
            }
        }
    }
}