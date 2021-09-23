using System;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Caching.Extensions;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Domain;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Services;
using HumanaEdge.Webcore.Example.Models.Immutable;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Example.Domain
{
    /// <summary>
    /// A service for retrieving cat facts.
    /// </summary>
    [DiComponent]
    internal sealed class CatFactsService
        : ICatFactsService
    {
        /// <summary>
        /// The cache key for cat facts.
        /// </summary>
        internal const string CatFactsKey = "CatFactsKey";

        private readonly IDistributedCache _cache;

        private readonly IRandomCatFactService _randomCatFactService;

        private readonly ILogger<CatFactsService> _logger;

        private readonly DistributedCacheEntryOptions _cacheOptions =
            new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="cache">Cache.</param>
        /// <param name="randomCatFactService">A service for getting random cat facts. </param>
        /// <param name="logger">The app logger. </param>
        public CatFactsService(
            IDistributedCache cache,
            IRandomCatFactService randomCatFactService,
            ILogger<CatFactsService> logger)
        {
            _cache = cache;
            _randomCatFactService = randomCatFactService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<CatFact?> GetAsync(CancellationToken cancellationToken, bool forceRefresh = false)
        {
            Func<CancellationToken, Task<CatFact?>> catFactory = async (ct) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10), ct);
#if NET5_0_OR_GREATER
                return new CatFact { Text = "Cat's toes are called beans" };
#else
                return new CatFact("Cat's toes are called beans");
#endif
            };
            if (forceRefresh)
            {
                _logger.LogInformation("Force Refresh cache.");
                var request = await catFactory.Invoke(cancellationToken);
                await _cache.SetAsync(CatFactsKey, request!, cancellationToken, _cacheOptions);
                return request;
            }

            _logger.LogInformation("Get or Create cache");
            return await _cache.GetOrCreateAsync(CatFactsKey, catFactory, cancellationToken, _cacheOptions);
        }
    }
}