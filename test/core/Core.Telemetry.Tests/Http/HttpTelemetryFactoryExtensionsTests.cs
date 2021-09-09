using System;
using AutoFixture;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using HumanaEdge.Webcore.Core.Testing;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Core.Telemetry.Tests.Http
{
    /// <summary>
    /// Unit tests for the <see cref="HttpTelemetryFactoryExtensions" /> class.
    /// </summary>
    public class HttpTelemetryFactoryExtensionsTests : BaseTests
    {
        /// <summary>
        /// Mock telemetry factory.
        /// </summary>
        private readonly Mock<ITelemetryFactory> _mockTelemetryFactory;

        /// <summary>
        /// Common setup code for the tests.
        /// </summary>
        public HttpTelemetryFactoryExtensionsTests()
        {
            _mockTelemetryFactory = Moq.Create<ITelemetryFactory>();
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="HttpTelemetryFactoryExtensions.TrackDependencyHttpTelemetry" /> method.
        /// </summary>
        [Fact]
        public void TrackDependencyHttpTelemetryTest()
        {
            // arrange
            var fakeStartTime = FakeData.Create<DateTimeOffset>();
            var fakeDuration = FakeData.Create<double>();
            var fakeHttpMethod = FakeData.Create<string>();
            var fakeUri = FakeData.Create<string>();
            var fakeSuccess = FakeData.Create<bool>();
            var fakeTelemetryConfiguration = FakeData.Create<TelemetryConfiguration>();
            var fakeResultCode = FakeData.Create<string>();
            var fakeAlert = FakeData.Create<bool>();

            var expectedDependencyTelemetry = new DependencyHttpTelemetry(
                fakeStartTime,
                fakeDuration,
                fakeResultCode,
                fakeHttpMethod,
                fakeUri,
                fakeSuccess,
                fakeTelemetryConfiguration,
                fakeAlert);

            var expectedTelemetryEvent = expectedDependencyTelemetry.ToTelemetryEvent();

            _mockTelemetryFactory.Setup(fac => fac.Track(expectedTelemetryEvent));

            // act
            _mockTelemetryFactory.Object.TrackDependencyHttpTelemetry(
                fakeStartTime,
                fakeDuration,
                fakeResultCode,
                fakeHttpMethod,
                fakeUri,
                fakeAlert,
                fakeTelemetryConfiguration,
                fakeSuccess);
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="HttpTelemetryFactoryExtensions.TrackRequestHttpTelemetry" /> method.
        /// </summary>
        [Fact]
        public void TrackRequestHttpTelemetryTest()
        {
            // arrange
            var fakeStartTime = FakeData.Create<DateTimeOffset>();
            var fakeDuration = FakeData.Create<double>();
            var fakeHttpMethod = FakeData.Create<string>();
            var fakeUri = FakeData.Create<string>();
            var fakeSuccess = FakeData.Create<bool>();
            var fakeTelemetryConfiguration = FakeData.Create<TelemetryConfiguration>();
            var fakeResultCode = FakeData.Create<string>();
            var fakeAlert = FakeData.Create<bool>();

            var expectedRequestHttpTelemetry = new RequestHttpTelemetry(
                fakeStartTime,
                fakeDuration,
                fakeResultCode,
                fakeHttpMethod,
                fakeUri,
                fakeSuccess,
                fakeTelemetryConfiguration,
                fakeAlert);

            var expectedTelemetryEvent = expectedRequestHttpTelemetry.ToTelemetryEvent();

            _mockTelemetryFactory.Setup(fac => fac.Track(expectedTelemetryEvent));

            // act
            _mockTelemetryFactory.Object.TrackRequestHttpTelemetry(
                fakeStartTime,
                fakeDuration,
                fakeResultCode,
                fakeHttpMethod,
                fakeUri,
                fakeAlert,
                fakeTelemetryConfiguration,
                fakeSuccess);
        }
    }
}