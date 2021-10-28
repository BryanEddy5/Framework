using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.AccessTokens;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests.AccessTokens
{
    /// <summary>
    /// Unit tests for <see cref="AccessTokenCacheService"/>.
    /// </summary>
    public class AccessTokenCacheServiceTests : BaseTests
    {
        private const string CacheValue = "bar";

        private const string CacheKey = "foo";

        private IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        private Mock<ILogger<AccessTokenCacheService>> _loggerMock;

        private AccessTokenCacheService _accessTokenCacheService;

        private Func<CancellationToken, Task<string>> _tokenFactory = token => Task.FromResult(CacheValue);

        /// <summary>
        /// Common test setup.
        /// </summary>
        public AccessTokenCacheServiceTests()
        {
            _loggerMock = Moq.Create<ILogger<AccessTokenCacheService>>(MockBehavior.Loose);
            _accessTokenCacheService = new AccessTokenCacheService(
                _loggerMock.Object,
                _memoryCache);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="IAccessTokenCacheService.GetAsync"/> for a cache hit.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetAsync_CacheHit()
        {
            // arrange
            await _accessTokenCacheService.GetAsync(
                _tokenFactory,
                CacheKey,
                CancellationTokenSource.Token);

            // act
            var actual = await _accessTokenCacheService.GetAsync(
                _tokenFactory,
                CacheKey,
                CancellationTokenSource.Token);

            // assert
            actual.Should().Be(CacheValue);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="IAccessTokenCacheService.GetAsync"/> for a cache hit during a forced refresh.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetAsync_CacheHit_ForceRefresh()
        {
            // arrange
            await _accessTokenCacheService.GetAsync(
                _tokenFactory,
                CacheKey,
                CancellationTokenSource.Token,
                true);

            // act
            var actual = await _accessTokenCacheService.GetAsync(
                _tokenFactory,
                CacheKey,
                CancellationTokenSource.Token);

            // assert
            actual.Should().Be(CacheValue);
        }
    }
}