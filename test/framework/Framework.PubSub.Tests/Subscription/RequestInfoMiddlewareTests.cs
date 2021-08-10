using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.PubSub.Subscription;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.PubSub;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Context;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.Middleware;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Subscription
{
    /// <summary>
    /// Unit tests for <see cref="RequestInfoMiddleware{TMessage}"/>.
    /// </summary>
    public class RequestInfoMiddlewareTests : BaseTests
    {
        /// <summary>
        ///     SUT.
        /// </summary>
        private readonly RequestInfoMiddleware<string> _requestInfoMiddleware;

        /// <summary>
        ///     Instance of logger. <see cref="ILogger" />.
        /// </summary>
        private readonly Mock<ITelemetryFactory> _telemetryFactoryMock;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RequestInfoMiddlewareTests()
        {
            _telemetryFactoryMock = Moq.Create<ITelemetryFactory>();
            _requestInfoMiddleware = new RequestInfoMiddleware<string>(_telemetryFactoryMock.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when an exception is thrown.  The telemetry
        /// should reflect that the event was unsuccessful.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task FailedTelemetryWhenExceptionIsThrown()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            MessageDelegate next = str => throw new ArgumentException("This should never run");
            _telemetryFactoryMock.Setup(
                x => x.Track(
                    It.Is<TelemetryEvent>(
                        t => t.Name == "SubscriptionTelemetry" &&
                             t.TelemetryType == TelemetryType.Subscription &&
                             (string)t.Tags[nameof(PubSubTelemetry.MessageId)] == fakeContext.MessageId &&
                             (bool)t.Tags[nameof(PubSubTelemetry.Success)] == false)));

            // act
            var actual = new Func<Task>(async () => await _requestInfoMiddleware.NextAsync(fakeContext, next));

            // assert
            await actual.Should().ThrowExactlyAsync<ArgumentException>();
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionHandlingMiddleware{TMessage}.NextAsync"/> when an exception is thrown.  The telemetry
        /// should reflect that the event was unsuccessful.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SuccessfulTelemetry_NoExceptions()
        {
            // arrange
            var fakeContext = FakeData.Create<SubscriptionContext>();
            MessageDelegate next = str => Task.CompletedTask;
            _telemetryFactoryMock.Setup(
                x => x.Track(
                    It.Is<TelemetryEvent>(
                        t => t.Name == "SubscriptionTelemetry" &&
                             t.TelemetryType == TelemetryType.Subscription &&
                             (string)t.Tags[nameof(PubSubTelemetry.MessageId)] == fakeContext.MessageId &&
                             (bool)t.Tags[nameof(PubSubTelemetry.Success)] == true)));

            // act + assert
            await _requestInfoMiddleware.NextAsync(fakeContext, next);
        }
    }
}