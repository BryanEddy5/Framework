using AutoFixture;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using HumanaEdge.Webcore.Core.Testing;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Core.Telemetry.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="TelemetryFactoryExtensions" /> class.
    /// </summary>
    public class TelemetryFactoryExtensionsTests : BaseTests
    {
        /// <summary>
        /// Mock telemetry factory.
        /// </summary>
        private readonly Mock<ITelemetryFactory> _mockTelemetryFactory;

        /// <summary>
        /// Common setup code for the tests.
        /// </summary>
        public TelemetryFactoryExtensionsTests()
        {
            _mockTelemetryFactory = Moq.Create<ITelemetryFactory>();
        }

        /// <summary>
        /// Verifies the behavior of the <see cref="HttpTelemetryFactoryExtensions.TrackRequestHttpTelemetry" /> method.
        /// </summary>
        [Fact]
        public void TrackCustomTelemetryTest()
        {
            // arrange
            var fakeName = FakeData.Create<string>();
            var fakeTelemetryConfiguration = FakeData.Create<TelemetryConfiguration>();
            var fakeAlert = FakeData.Create<bool>();

            var expectedCustomTelemetry = new CustomTelemetry(
                fakeName,
                fakeTelemetryConfiguration,
                fakeAlert);

            var expectedTelemetryEvent = expectedCustomTelemetry.ToTelemetryEvent();

            // manually checking properties to avoid comparing the timestamp
            _mockTelemetryFactory.Setup(
                fac => fac.Track(
                    It.Is<TelemetryEvent>(
                        telemetry =>
                            telemetry.Name == expectedTelemetryEvent.Name &&
                            telemetry.TelemetryType == TelemetryType.Custom &&
                            ReferenceEquals(telemetry.Tags, expectedCustomTelemetry.Tags))));

            // act
            _mockTelemetryFactory.Object.TrackCustomTelemetry(
                fakeName,
                fakeTelemetryConfiguration,
                fakeAlert);
        }
    }
}