using System;
using System.Collections.Generic;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Telemetry.Sinks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Telemetry.Tests.Sinks
{
    /// <summary>
    /// Test for Telemetry Sinks.
    /// </summary>
    public class LoggerSinkTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly LoggerSink _sink;

        /// <summary>
        /// Setup mocks used for testing.
        /// </summary>
        public LoggerSinkTests()
        {
            // Set behavior to default
            var mockLogger = Moq.Create<ILogger<LoggerSink>>(MockBehavior.Loose);
            _sink = new LoggerSink(mockLogger.Object);
        }

        /// <summary>
        /// Ensure no exceptions are thrown when invoking the EmitTelemetry method for sinks.
        /// </summary>
        [Fact]
        public void LoggerSinkEmitNoExceptions()
        {
            // arrange
            var fakeTelemetryEvent = new TelemetryEvent(
                "test",
                TelemetryType.Custom,
                DateTimeOffset.UtcNow,
                new Dictionary<string, object>());

            // act
            _sink.Emit(fakeTelemetryEvent);
        }
    }
}