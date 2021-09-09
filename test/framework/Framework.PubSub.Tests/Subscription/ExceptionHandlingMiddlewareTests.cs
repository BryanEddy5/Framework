using System;
using System.Threading.Tasks;
using AutoFixture;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Storage;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling;
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

        private Mock<IExceptionStorageService> _storageClientMock;

        private SubscriptionContext _fakeContext;

        private PubsubMessage _fakePubSubMessage;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public ExceptionHandlingMiddlewareTests()
        {
            _fakeContext = FakeData.Create<SubscriptionContext>();
            _fakePubSubMessage = FakeData.Create<PubsubMessage>();
            _fakeContext.Items[ContextKeys.SubscriptionContextKey] = _fakePubSubMessage;
            _loggerMock = Moq.Create<ILogger<ExceptionHandlingMiddleware<string>>>(MockBehavior.Loose);
            _storageClientMock = Moq.Create<IExceptionStorageService>();
            _exceptionHandlingMiddleware = new ExceptionHandlingMiddleware<string>(
                _loggerMock.Object,
                _storageClientMock.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when an exception is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task RethrowsException()
        {
            // arrange
            var exception = new ArgumentException("This should never run");
            MessageDelegate next = str => throw exception;
            _storageClientMock.Setup(
                    x => x.LoadException<string>(
                        _fakePubSubMessage.Data.ToStringUtf8(),
                        exception,
                        _fakeContext.RequestCancelledToken))
                .Returns(Task.CompletedTask);

            // assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _exceptionHandlingMiddleware.NextAsync(_fakeContext, next));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when a <see cref="PubSubException"/> is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task ThrowsPubSubException()
        {
            // arrange
            var exception = new PubSubException("This should never run");
            MessageDelegate next = str => throw exception;
            _storageClientMock.Setup(
                    x => x.LoadException<string>(
                        _fakePubSubMessage.Data.ToStringUtf8(),
                        exception,
                        _fakeContext.RequestCancelledToken))
                .Returns(Task.CompletedTask);

            // assert
            await Assert.ThrowsAsync<PubSubException>(
                () => _exceptionHandlingMiddleware.NextAsync(_fakeContext, next));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when a <see cref="JsonException"/> is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SerializationExceptionThrown()
        {
            // arrange
            var exception = new JsonException();
            MessageDelegate next = str => throw exception;
            _storageClientMock.Setup(
                    x => x.LoadException<string>(
                        _fakePubSubMessage.Data.ToStringUtf8(),
                        exception,
                        _fakeContext.RequestCancelledToken))
                .Returns(Task.CompletedTask);

            // assert
            await Assert.ThrowsAsync<JsonParsingException>(
                () => _exceptionHandlingMiddleware.NextAsync(_fakeContext, next));
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