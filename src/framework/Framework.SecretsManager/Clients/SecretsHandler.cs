using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.SecretsManager;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Clients
{
    /// <inheritdoc />
    internal sealed class SecretsHandler : ISecretsHandler
    {
        /// <summary>
        /// A client for retrieving the stored secret.
        /// </summary>
        private readonly ISecretsClient _secretsClient;

        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="secretsClient">The client for retrieving secrets.</param>
        /// <param name="memoryCache">The in memory cache. </param>
        public SecretsHandler(ISecretsClient secretsClient, IMemoryCache memoryCache)
        {
            _secretsClient = secretsClient;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc />
        public async Task<TSecret> GetAsync<TSecret>(SecretsKey secretsKey, CancellationToken cancellationToken)
            where TSecret : ISecret
        {
            if (_memoryCache.TryGetValue(secretsKey, out var secret))
            {
                return (TSecret)secret;
            }

            secret = await _secretsClient.GetAsync<TSecret>(secretsKey, cancellationToken);
            _memoryCache.Set(secretsKey, secret, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(secretsKey.CacheExpirationInMinutesRelativeToNow)
            });

            return (TSecret)secret;
        }
    }
}