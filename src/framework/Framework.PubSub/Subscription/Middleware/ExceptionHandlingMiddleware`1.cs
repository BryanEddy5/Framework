using System;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware
{
    /// <summary>
    /// Handles exceptions thrown by subscribers.
    /// </summary>
    /// <typeparam name="TMessage"> The message shape. </typeparam>
    internal sealed class ExceptionHandlingMiddleware<TMessage> : ISubscriptionMiddleware<TMessage>
    {
        private readonly IExceptionStorageService _exceptionStorage;

        private readonly ILogger<ExceptionHandlingMiddleware<TMessage>> _logger;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="logger"> The application logger. </param>
        /// <param name="exceptionStorage"> A client for uploading objects to a storage bucket. </param>
        public ExceptionHandlingMiddleware(
            ILogger<ExceptionHandlingMiddleware<TMessage>> logger,
            IExceptionStorageService exceptionStorage)
        {
            _logger = logger;
            _exceptionStorage = exceptionStorage;
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

                var pubsubMessage = (PubsubMessage)subscriptionMessage.Items[ContextKeys.SubscriptionContextKey];

                await _exceptionStorage.LoadException<TMessage>(
                    pubsubMessage.Data.ToStringUtf8(),
                    exception,
                    subscriptionMessage.RequestCancelledToken);

                if (exception is JsonException)
                {
                    throw new JsonParsingException(exception);
                }

                throw;
            }
        }
    }
}