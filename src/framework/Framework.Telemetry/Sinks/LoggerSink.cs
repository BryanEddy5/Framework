using HumanaEdge.Webcore.Core.Telemetry;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Framework.Telemetry.Sinks
{
    /// <summary>
    /// Emits telemetry by utilizing the logger as a sink.
    /// </summary>
    internal sealed class LoggerSink : ITelemetrySink
    {
        /// <summary>
        /// The application logger to emit information to it's designated sinks.
        /// </summary>
        private readonly ILogger<LoggerSink> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerSink" /> class.
        /// </summary>
        /// <param name="logger">Injected logger.</param>
        public LoggerSink(ILogger<LoggerSink> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void Emit(TelemetryEvent telemetryEvent)
        {
            _logger.LogInformation("{@metricEvent}", telemetryEvent);
        }
    }
}