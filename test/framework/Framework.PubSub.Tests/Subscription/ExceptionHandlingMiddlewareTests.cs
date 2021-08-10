using System;
using System.Threading.Tasks;
using AutoFixture;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Subscription
{
    /// <summary>
    /// Unit tests for <see cref="ExceptionHandlingMiddleware{TMessage}"/>.
    /// </summary>
    public class ExceptionHandlingMiddlewareTests : BaseTests
    {
        /// <summary>
        ///     SUT.
        /// </summary>
        private ExceptionHandlingMiddleware<string> _exceptionHandlingMiddleware;

        /// <summary>
        ///     Instance of logger. <see cref="ILogger" />.
        /// </summary>
        private Mock<ILogger<ExceptionHandlingMiddleware<string>>> _loggerMock;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public ExceptionHandlingMiddlewareTests()
        {
            _loggerMock = Moq.Create<ILogger<ExceptionHandlingMiddleware<string>>>(MockBehavior.Loose);
            _exceptionHandlingMiddleware = new ExceptionHandlingMiddleware<string>(_loggerMock.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when an exception is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task RethrowsException()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            MessageDelegate next = str => throw new ArgumentException("This should never run");

            // assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _exceptionHandlingMiddleware.NextAsync(fakeContext, next));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when a <see cref="PubSubException"/> is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task ThrowsPubSubException()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            MessageDelegate next = str => throw new PubSubException("This should never run");

            // assert
            await Assert.ThrowsAsync<PubSubException>(
                () => _exceptionHandlingMiddleware.NextAsync(fakeContext, next));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when a <see cref="JsonException"/> is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SerializationExceptionThrown()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            MessageDelegate next = str => throw new JsonException();

            // assert
            await Assert.ThrowsAsync<JsonParsingException>(
                () => _exceptionHandlingMiddleware.NextAsync(fakeContext, next));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when NO exception ist thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task NoExceptionThrown()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            MessageDelegate next = str => Task.CompletedTask;

            // act + assert
            await _exceptionHandlingMiddleware.NextAsync(fakeContext, next);
        }
    }
}