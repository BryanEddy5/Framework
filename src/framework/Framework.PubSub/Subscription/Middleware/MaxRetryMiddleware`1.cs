using System;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware
{
    /// <summary>
    /// Enforces the maximum amount of retries.
    /// </summary>
    /// <typeparam name="TMessage">The message shape.</typeparam>
    public class MaxRetryMiddleware<TMessage> : ISubscriptionMiddleware<TMessage>
    {
        private readonly IMemoryCache _memoryCache;

        private readonly IOptionsMonitor<PubSubOptions> _options;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="memoryCache">The in memory cache.</param>
        /// <param name="options">The configuration options.</param>
        public MaxRetryMiddleware(IMemoryCache memoryCache, IOptionsMonitor<PubSubOptions> options)
        {
            _memoryCache = memoryCache;
            _options = options;
        }

        /// <inheritdoc />
        public async Task NextAsync(ISubscriptionContext subscriptionMessage, MessageDelegate messageDelegate)
        {
            MaxRetryCheck(subscriptionMessage);
            await messageDelegate(subscriptionMessage);
        }

        private void MaxRetryCheck(ISubscriptionContext subscriptionMessage)
        {
            var options = _options.Get(typeof(TMessage).FullName);
            var cacheExpiry = TimeSpan.FromMinutes(options.Resiliency.RetryCacheExpirationInMinutes);
            var key = subscriptionMessage.MessageId + options.Name;

            if (!_memoryCache.TryGetValue<int>(key, out var entry))
            {
                entry = 1;
                _memoryCache.Set(key, entry, cacheExpiry);
            }
            else
            {
                _memoryCache.Set(key, Interlocked.Increment(ref entry), cacheExpiry);
            }

            if (entry > options.Resiliency.MaxRetries)
            {
                throw new MaxRetryExceededException(options.Resiliency.MaxRetries);
            }
        }
    }
}