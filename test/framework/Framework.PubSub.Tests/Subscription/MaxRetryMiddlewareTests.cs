using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Subscription
{
    /// <summary>
    /// Unit tests for <see cref="MaxRetryMiddleware{TMessage}"/>.
    /// </summary>
    public class MaxRetryMiddlewareTests : BaseTests
    {
        private Mock<IMemoryCache> _memoryCacheMock;

        private MaxRetryMiddleware<string> _maxRetryMiddleware;

        private Mock<IOptionsMonitor<PubSubOptions>> _optionsMock;

        private PubSubOptions _options;

        private MessageDelegate _messageDelegate = context => Task.CompletedTask;

        private SubscriptionContext _subscriptionMessage;

        private string _key;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public MaxRetryMiddlewareTests()
        {
            _memoryCacheMock = Moq.Create<IMemoryCache>();
            _optionsMock = Moq.Create<IOptionsMonitor<PubSubOptions>>();
            _options = FakeData.Build<PubSubOptions>().With(x => x.MaxRetries, 10).Create();
            _optionsMock.Setup(x => x.Get(typeof(string).FullName)).Returns(_options);
            _maxRetryMiddleware = new MaxRetryMiddleware<string>(_memoryCacheMock.Object, _optionsMock.Object);
            _subscriptionMessage = FakeData.Create<SubscriptionContext>();
            _key = _subscriptionMessage.MessageId + _options.Name;
        }

        /// <summary>
        /// Verifies the behavior of <see cref="MaxRetryMiddleware{TMessage}.NextAsync"/> when there is a cache miss.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task MaxRetryMiss()
        {
            // arrange
            object entry = 1;
            var cacheEntrySub = new CacheEntryStub(_key);
            _memoryCacheMock.Setup(x => x.TryGetValue(_key, out entry)).Returns(false);
            _memoryCacheMock.Setup(x => x.CreateEntry(_key)).Returns(cacheEntrySub);

            // act
            await _maxRetryMiddleware.NextAsync(_subscriptionMessage, _messageDelegate);

            // assert
            cacheEntrySub.Value.Should().Be(entry);
            cacheEntrySub.Key.Should().Be(_key);
            cacheEntrySub.AbsoluteExpirationRelativeToNow.Should().Be(MaxRetryMiddleware<string>.CacheExpiry);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="MaxRetryMiddleware{TMessage}.NextAsync"/> when there is a cache hit.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task MaxRetryHit()
        {
            // arrange
            object entry = 1;
            object expectedEntry = 2;
            var cacheEntrySub = new CacheEntryStub(_key);
            _memoryCacheMock.Setup(x => x.TryGetValue(_key, out entry)).Returns(true);
            _memoryCacheMock.Setup(x => x.CreateEntry(_key)).Returns(cacheEntrySub);

            // act
            await _maxRetryMiddleware.NextAsync(_subscriptionMessage, _messageDelegate);

            // assert
            cacheEntrySub.Value.Should().Be(expectedEntry);
            cacheEntrySub.Key.Should().Be(_key);
            cacheEntrySub.AbsoluteExpirationRelativeToNow.Should().Be(MaxRetryMiddleware<string>.CacheExpiry);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="MaxRetryMiddleware{TMessage}.NextAsync"/> when max retries has been exceeded.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task MaxRetry_Exception()
        {
            // arrange
            object entry = _options.MaxRetries;
            object expected = (int)entry + 1;
            var cacheEntrySub = new CacheEntryStub(_key);
            _memoryCacheMock.Setup(x => x.TryGetValue(_key, out entry)).Returns(true);
            _memoryCacheMock.Setup(x => x.CreateEntry(_key)).Returns(cacheEntrySub);

            // act
            var actual = new Func<Task>(
                async () => await _maxRetryMiddleware.NextAsync(_subscriptionMessage, _messageDelegate));

            // assert
            await actual.Should().ThrowExactlyAsync<MaxRetryExceededException>();
            cacheEntrySub.Value.Should().Be(expected);
            cacheEntrySub.Key.Should().Be(_key);
            cacheEntrySub.AbsoluteExpirationRelativeToNow.Should().Be(MaxRetryMiddleware<string>.CacheExpiry);
            try
            {
                await actual.Invoke();
            }
            catch (MaxRetryExceededException exception)
            {
                exception.Reply.Should().BeEquivalentTo(Reply.Ack);
            }
        }

        private class CacheEntryStub : ICacheEntry
        {
            public CacheEntryStub(string key)
            {
                Key = key;
            }

            public object Key { get; }

            public object Value { get; set; }

            public DateTimeOffset? AbsoluteExpiration { get; set; }

            public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

            public TimeSpan? SlidingExpiration { get; set; }

            public IList<IChangeToken> ExpirationTokens { get; }

            public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; }

            public CacheItemPriority Priority { get; set; }

            public long? Size { get; set; }

            public void Dispose()
            {
            }
        }
    }
}