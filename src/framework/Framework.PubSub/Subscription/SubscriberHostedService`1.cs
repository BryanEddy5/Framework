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
using Microsoft.Extensions.Options;

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

        private readonly IOptionsMonitor<PubSubOptions> _options;

        private readonly MessageDelegate _messageDelegate;

        private readonly IActivityFactory _activityFactory;

        private SubscriberClient? _subscriber;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="subscriberClientFactory">A factory that generates a <see cref="SubscriberClient" />.</param>
        /// <param name="middlewareBuilder">Creates the pipeline and instruments the middleware. </param>
        /// <param name="options">The configuration options.</param>
        /// <param name="activityFactory">A factory for creating a new activity with W3C trace context.</param>
        public SubscriberHostedService(
            ILogger<SubscriberHostedService<TMessage>> logger,
            ISubscriberClientFactory subscriberClientFactory,
            IMiddlewareBuilder<TMessage> middlewareBuilder,
            IOptionsMonitor<PubSubOptions> options,
            IActivityFactory activityFactory = null!)
        {
            _activityFactory = activityFactory;
            _logger = logger;
            _subscriberClientFactory = subscriberClientFactory;
            _options = options;
            _messageDelegate = middlewareBuilder.Build();
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber = await _subscriberClientFactory.Create(typeof(TMessage).FullName!);
            var options = _options.Get(typeof(TMessage).FullName);

            _ = _subscriber.StartAsync(
                async (message, cancel) =>
                {
                    if (options.ImmediatelyAckMessage)
                    {
#pragma warning disable CS4014
                        Task.Run(() => ExecuteAsync(message, cancel), cancellationToken);
                        return SubscriberClient.Reply.Ack;
#pragma warning restore CS4014
                    }

                    return await ExecuteAsync(message, cancel);
                });
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber?.StopAsync(cancellationToken)!;
        }

        private async Task<SubscriberClient.Reply> ExecuteAsync(PubsubMessage message, CancellationToken cancellationToken)
        {
            var activity = Activity.Current;
            try
            {
                activity = _activityFactory?.Create(message);
                var subscriptionContext = new SubscriptionContext(message.MessageId, cancellationToken)
                {
                    Items = { [ContextKeys.SubscriptionContextKey] = message }
                };
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