using System;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware
{
    /// <summary>
    /// Handles exceptions thrown by subscribers.
    /// </summary>
    /// <typeparam name="TMessage">The message shape.</typeparam>
    internal sealed class ExceptionHandlingMiddleware<TMessage> : ISubscriptionMiddleware<TMessage>
    {
        private readonly ILogger<ExceptionHandlingMiddleware<TMessage>> _logger;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware<TMessage>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task NextAsync(
            ISubscriptionContext subscriptionMessage,
            MessageDelegate messageDelegate)
        {
            try
            {
                await messageDelegate.Invoke(subscriptionMessage);
            }
            catch (JsonException exception)
            {
                throw new JsonParsingException(exception);
            }
            catch (Exception exception)
            {
                if (exception is PubSubException pubSubException)
                {
                    _logger.LogError(exception, pubSubException.LoggedMessage, pubSubException.Args);
                }
                else
                {
                    _logger.LogError(exception, "An exception was thrown");
                }

                throw;
            }
        }
    }
}