using System.Collections.Generic;
using AutoFixture;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Telemetry.Tests
{
    /// <summary>
    /// Testing the TelemetryFactory.
    /// </summary>
    public class TelemetryFactoryTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly ITelemetryFactory _factory;

        /// <summary>
        /// Sink for mock testing.
        /// </summary>
        private readonly Mock<ITelemetrySink> _sink1;

        /// <summary>
        /// Sink for testing.
        /// </summary>
        private readonly Mock<ITelemetrySink> _sink2;

        /// <summary>
        /// Test initialization.
        /// </summary>
        public TelemetryFactoryTests()
        {
            // arrange
            _sink1 = Moq.Create<ITelemetrySink>();
            _sink2 = Moq.Create<ITelemetrySink>();
            var sinks = new List<ITelemetrySink>
            {
                _sink1.Object,
                _sink2.Object
            };
            _factory = new TelemetryFactory(sinks);
        }

        /// <summary>
        /// Verifies that the factory dispatches each event to all available sinks.
        /// </summary>
        [Fact]
        public void TrackTest()
        {
            // arrange
            var fakeEvent = FakeData.Create<TelemetryEvent>();

            _sink1.Setup(s => s.Emit(fakeEvent));
            _sink2.Setup(s => s.Emit(fakeEvent));

            // act
            _factory.Track(fakeEvent);
        }
    }
}