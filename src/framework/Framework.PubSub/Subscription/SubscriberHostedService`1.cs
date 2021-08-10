using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Factory;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware.Builder;
using HumanaEdge.Webcore.Framework.PubSub.TraceContext;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription
{
    /// <summary>
    /// A hosted service for listening to a pubsub subscription.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to be received from the subscription.</typeparam>
    internal class SubscriberHostedService<TMessage> : IHostedService
    {
        private readonly ILogger<SubscriberHostedService<TMessage>> _logger;

        private readonly ISubscriberClientFactory _subscriberClientFactory;

        private readonly MessageDelegate _messageDelegate;

        private readonly IActivityFactory _activityFactory;

        private SubscriberClient? _subscriber;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="subscriberClientFactory">A factory that generates a <see cref="SubscriberClient" />.</param>
        /// <param name="middlewareBuilder">Creates the pipeline and instruments the middleware. </param>
        /// <param name="activityFactory">A factory for creating a new activity with W3C trace context.</param>
        public SubscriberHostedService(
            ILogger<SubscriberHostedService<TMessage>> logger,
            ISubscriberClientFactory subscriberClientFactory,
            IMiddlewareBuilder<TMessage> middlewareBuilder,
            IActivityFactory activityFactory = null!)
        {
            _activityFactory = activityFactory;
            _logger = logger;
            _subscriberClientFactory = subscriberClientFactory;
            _messageDelegate = middlewareBuilder.Build();
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber = await _subscriberClientFactory.Create(typeof(TMessage).FullName!);

            _ = _subscriber.StartAsync(
                async (message, cancel) =>
                {
                    var activity = Activity.Current;
                    try
                    {
                        activity = _activityFactory?.Create(message);
                        var subscriptionContext = new SubscriptionContext(message.MessageId, cancel);
                        subscriptionContext.Items[ContextKeys.SubscriptionContextKey] = message;
                        await _messageDelegate.Invoke(subscriptionContext);

                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception exception)
                    {
                        return HandleException(exception);
                    }
                    finally
                    {
                        activity?.Stop();
                    }
                });
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber?.StopAsync(cancellationToken)!;
        }

        private SubscriberClient.Reply HandleException(Exception exception)
        {
            var reply = Reply.Nack;
            if (exception is PubSubException pubSubException)
            {
                reply = pubSubException.Reply;
            }

            return (int)reply == (int)Reply.Ack ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack;
        }
    }
}