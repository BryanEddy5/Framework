using System;
using AutoFixture;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using HumanaEdge.Webcore.Core.Telemetry.PubSub;
using HumanaEdge.Webcore.Core.Testing;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Core.Telemetry.Tests.Http
{
    /// <summary>
    /// Unit tests for the <see cref="HttpTelemetryFactoryExtensions" /> class.
    /// </summary>
    public class PubSubTelemetryFactoryExtensionsTests : BaseTests
    {
        /// <summary>
        /// Mock telemetry factory.
        /// </summary>
        private readonly Mock<ITelemetryFactory> _mockTelemetryFactory;

        /// <summary>
        /// Common setup code for the tests.
        /// </summary>
        public PubSubTelemetryFactoryExtensionsTests()
        {
            _mockTelemetryFactory = Moq.Create<ITelemetryFactory>();
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="PubSubTelemetryFactoryExtensions.TrackSubscriptionTelemetry" /> method.
        /// </summary>
        [Fact]
        public void TrackSubscriptionTelemetryTest()
        {
            // arrange
            var fakeStartTime = FakeData.Create<DateTimeOffset>();
            var fakeMessageId = FakeData.Create<string>();
            var fakeDuration = FakeData.Create<double>();
            var fakeSuccess = FakeData.Create<bool>();
            var fakeTelemetryConfiguration = FakeData.Create<TelemetryConfiguration>();
            var fakeAlert = FakeData.Create<bool>();

            var expectedDependencyTelemetry = new PubSubTelemetry(
                "SubscriptionTelemetry",
                TelemetryType.Subscription,
                fakeStartTime,
                fakeMessageId,
                fakeDuration,
                fakeSuccess,
                fakeTelemetryConfiguration,
                fakeAlert);

            var expectedTelemetryEvent = expectedDependencyTelemetry.ToTelemetryEvent();

            _mockTelemetryFactory.Setup(fac => fac.Track(expectedTelemetryEvent));

            // act
            _mockTelemetryFactory.Object.TrackSubscriptionTelemetry(
                fakeStartTime,
                fakeMessageId,
                fakeDuration,
                fakeSuccess,
                fakeTelemetryConfiguration,
                fakeAlert);
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="PubSubTelemetryFactoryExtensions.TrackPublicationTelemetry" /> method.
        /// </summary>
        [Fact]
        public void TrackPublicationTelemetryTest()
        {
            // arrange
            var fakeStartTime = FakeData.Create<DateTimeOffset>();
            var fakeMessageId = FakeData.Create<string>();
            var fakeDuration = FakeData.Create<double>();
            var fakeSuccess = FakeData.Create<bool>();
            var fakeTelemetryConfiguration = FakeData.Create<TelemetryConfiguration>();
            var fakeAlert = FakeData.Create<bool>();

            var expectedDependencyTelemetry = new PubSubTelemetry(
                "PublicationTelemetry",
                TelemetryType.Publication,
                fakeStartTime,
                fakeMessageId,
                fakeDuration,
                fakeSuccess,
                fakeTelemetryConfiguration,
                fakeAlert);

            var expectedTelemetryEvent = expectedDependencyTelemetry.ToTelemetryEvent();

            _mockTelemetryFactory.Setup(fac => fac.Track(expectedTelemetryEvent));

            // act
            _mockTelemetryFactory.Object.TrackPublicationTelemetry(
                fakeStartTime,
                fakeMessageId,
                fakeDuration,
                fakeSuccess,
                fakeTelemetryConfiguration,
                fakeAlert);
        }
    }
}