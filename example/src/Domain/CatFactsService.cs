using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Caching.Extensions;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Core.Telemetry;
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

        private readonly ITelemetryFactory _factory;

        private readonly DistributedCacheEntryOptions _cacheOptions =
            new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(20));

        private readonly Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="cache">Cache.</param>
        /// <param name="randomCatFactService">A service for getting random cat facts. </param>
        /// <param name="logger">The app logger. </param>
        /// <param name="factory">The telemetry factory. </param>
        public CatFactsService(
            IDistributedCache cache,
            IRandomCatFactService randomCatFactService,
            ILogger<CatFactsService> logger,
            ITelemetryFactory factory)
        {
            _cache = cache;
            _randomCatFactService = randomCatFactService;
            _logger = logger;
            _factory = factory;
        }

        /// <inheritdoc />
        public async Task<CatFact?> GetAsync(CancellationToken cancellationToken, bool forceRefresh = false)
        {
            Func<CancellationToken, Task<CatFact?>> catFactory = async (ct) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
#if NET5_0_OR_GREATER
                return new CatFact { Text = "Cat's toes are called beans" };
#else
                return new CatFact("Cat's toes are called beans");
#endif
            };
            CatFact? response;
            _stopwatch.Start();
            if (forceRefresh)
            {
                _logger.LogInformation("Force Refresh cache");
                response = await catFactory.Invoke(cancellationToken);
                await _cache.SetAsync(CatFactsKey, response!, cancellationToken, _cacheOptions);
            }
            else
            {
                _logger.LogInformation("Get or Create cache");
                response = await _cache.GetOrCreateAsync(CatFactsKey, catFactory, cancellationToken, _cacheOptions);
            }

            _stopwatch.Stop();
            GetTelemetry(response, _stopwatch.ElapsedMilliseconds);
            _stopwatch.Reset();

            return response;
        }

        private void GetTelemetry(CatFact? catFact, long duration)
        {
            var dic = new TelemetryConfiguration();
            dic.Tags.Add("Response", catFact!);
            dic.Tags.Add("Duration", duration);
            dic.Tags.Add("Foo", "Bar");
            _factory.Track(
                new CustomTelemetry("Cat Facts Cache", dic, false));
        }
    }
}