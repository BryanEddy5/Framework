using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Caching.Extensions;
using HumanaEdge.Webcore.Core.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit;

namespace HumanaEdge.Webcore.Core.Caching.Tests
{
    /// <summary>
    /// Unit tests for <see cref="Extensions.DistributedCacheExtensions"/>.
    /// </summary>
    public class DistributedCacheExtensionsTest : BaseTests
    {
        private const string Key = "key";

        private readonly Foo _fakeFoo;

        private readonly Func<CancellationToken, Task<Foo>> _fooFactory;

        private readonly MemoryDistributedCache _cache;

        /// <summary>
        /// Common setup.
        /// </summary>
        public DistributedCacheExtensionsTest()
        {
            _fakeFoo = FakeData.Create<Foo>();
            _fooFactory = x => Task.FromResult(_fakeFoo);
            var options = new MemoryDistributedCacheOptions();
            var optionsMonitor = Moq.Create<IOptions<MemoryDistributedCacheOptions>>();
            optionsMonitor.Setup(x => x.Value).Returns(options);
            _cache = new MemoryDistributedCache(optionsMonitor.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="Extensions.DistributedCacheExtensions.GetOrCreateAsync{T}(IDistributedCache, string, Func{CancellationToken, Task{T}},CancellationToken)"/>
        /// cache miss.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetOrCreateAsyncCacheMiss()
        {
            // arrange

            // act
            await _cache.GetOrCreateAsync(Key, _fooFactory, CancellationTokenSource.Token);
            var result = await _cache.GetStringAsync(Key);

            // assert
            result.Should().BeEquivalentTo(JsonConvert.SerializeObject(_fakeFoo));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="Extensions.DistributedCacheExtensions.GetOrCreateAsync{T}(IDistributedCache, string, Func{CancellationToken, Task{T}},CancellationToken)"/>
        /// cache hit.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetOrCreateAsyncCacheHit()
        {
            // arrange
            await _cache.SetStringAsync(Key, JsonConvert.SerializeObject(_fakeFoo));

            // act
            await _cache.GetOrCreateAsync(Key, _fooFactory, CancellationTokenSource.Token);
            var result = await _cache.GetStringAsync(Key);

            // assert
            result.Should().BeEquivalentTo(JsonConvert.SerializeObject(_fakeFoo));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="Extensions.DistributedCacheExtensions.GetOrCreateAsync{T}(IDistributedCache, string, Func{CancellationToken, Task{T}},CancellationToken, DistributedCacheEntryOptions)"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetOrCreateAsyncWithOptions()
        {
            // arrange

            // act
            await _cache.GetOrCreateAsync(Key, _fooFactory, CancellationTokenSource.Token, new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
            var result = await _cache.GetStringAsync(Key);

            // assert
            result.Should().BeEquivalentTo(JsonConvert.SerializeObject(_fakeFoo));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="Extensions.DistributedCacheExtensions.SetAsync(IDistributedCache, string, object,CancellationToken)"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SetAsync()
        {
            // arrange

            // act
            await _cache.SetAsync(Key, _fakeFoo, CancellationTokenSource.Token);
            var result = await _cache.GetStringAsync(Key);

            // assert
            result.Should().BeEquivalentTo(JsonConvert.SerializeObject(_fakeFoo));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="Extensions.DistributedCacheExtensions.SetAsync(IDistributedCache, string, object,CancellationToken, DistributedCacheEntryOptions)"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SetAsyncWithOptions()
        {
            // arrange

            // act
            await _cache.SetAsync(
                Key,
                _fakeFoo,
                CancellationTokenSource.Token,
                new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1) });
            var result = await _cache.GetStringAsync(Key);

            // assert
            result.Should().BeEquivalentTo(JsonConvert.SerializeObject(_fakeFoo));
        }

        private class Foo
        {
            public string Name { get; set; }
        }
    }
}