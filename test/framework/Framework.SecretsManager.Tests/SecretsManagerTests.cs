using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.SecretsManager;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.SecretsManager.Clients;
using HumanaEdge.Webcore.Framework.SecretsManager.Tests.Stubs;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Tests
{
    /// <summary>
    /// Unit tests for <see cref="SecretsManager"/>.
    /// </summary>
    public class SecretsManagerTests : BaseTests
    {
        private Mock<ISecretsClient> _internalSecretsClientMock;

        private IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Common test setup.
        /// </summary>
        public SecretsManagerTests()
        {
            _internalSecretsClientMock = Moq.Create<ISecretsClient>();
        }

        /// <summary>
        /// Validates the behavior of <see cref="SecretsHandler.GetAsync{TSecret}(SecretsKey, CancellationToken)"/>
        /// during a cache hit.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetAsync_CacheHit()
        {
            // arrange
            var secretsHandler = new SecretsHandler(_internalSecretsClientMock.Object, _memoryCache);
            var fakeSecretsKey = FakeData.Create<SecretsKey>();
            var expected = FakeData.Create<FakeSecret>();
            Expression<Func<ISecretsClient, Task<FakeSecret>>> expression = x =>
                x.GetAsync<FakeSecret>(fakeSecretsKey, CancellationTokenSource.Token);
            _internalSecretsClientMock.Setup(expression).ReturnsAsync(expected);

            // act
            var actual1 = await secretsHandler.GetAsync<FakeSecret>(fakeSecretsKey, CancellationTokenSource.Token);
            var actual2 = await secretsHandler.GetAsync<FakeSecret>(fakeSecretsKey, CancellationTokenSource.Token);

            // assert
            actual1.Should().BeEquivalentTo(expected);
            actual2.Should().BeEquivalentTo(expected);
            _internalSecretsClientMock.Verify(expression, Times.Once);
        }

        /// <summary>
        /// Validates the behavior of <see cref="SecretsHandler.GetAsync{TSecret}(SecretsKey, CancellationToken)"/>
        /// during a cache miss.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetAsync_CacheMiss()
        {
            // arrange
            var secretsHandler = new SecretsHandler(_internalSecretsClientMock.Object, _memoryCache);
            var fakeSecretsKey = FakeData.Create<SecretsKey>();
            var expected = FakeData.Create<FakeSecret>();
            Expression<Func<ISecretsClient, Task<FakeSecret>>> expression = x =>
                x.GetAsync<FakeSecret>(fakeSecretsKey, CancellationTokenSource.Token);
            _internalSecretsClientMock.Setup(expression).ReturnsAsync(expected);

            // act
            var actual = await secretsHandler.GetAsync<FakeSecret>(fakeSecretsKey, CancellationTokenSource.Token);

            // assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}