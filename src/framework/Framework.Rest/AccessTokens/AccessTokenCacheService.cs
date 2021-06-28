using System;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Framework.Rest.AccessTokens
{
    /// <inheritdoc cref="IAccessTokenCacheService"/>
    internal sealed class AccessTokenCacheService : IAccessTokenCacheService
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// The application logger.
        /// </summary>
        private readonly ILogger<AccessTokenCacheService> _logger;

        private readonly IMemoryCache _cache;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="cache">Memory Cache.</param>
        public AccessTokenCacheService(
            ILogger<AccessTokenCacheService> logger,
            IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        /// <inheritdoc />
        public async Task<string> GetAsync(
            Func<CancellationToken, Task<string>> tokenFactory,
            string tokenKey,
            CancellationToken cancellationToken,
            bool forceRefresh = false)
        {
            if (!forceRefresh && _cache.TryGetValue(tokenKey, out var token))
            {
                _logger.LogInformation("Using cached token");
                return (string)token;
            }

            await SemaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                if (forceRefresh)
                {
                    _logger.LogInformation("Force refreshing token");
                    var accessToken = tokenFactory.Invoke(cancellationToken);
                    return await _cache.Set(tokenKey, accessToken, TimeSpan.FromDays(1));
                }

                _logger.LogInformation("Get or Create Token");
                return await _cache.GetOrCreateAsync(
                    tokenKey,
                    entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                        return tokenFactory.Invoke(cancellationToken);
                    });
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}