using HumanaEdge.Webcore.Core.Telemetry;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Framework.Telemetry.Sinks
{
    /// <summary>
    /// Emits telemetry by utilizing the logger as a sink.<br/>
    /// Exclusively for telemetry tagged as "Alert".
    /// </summary>
    internal sealed class AlertSink : ITelemetrySink
    {
        /// <summary>
        /// The tag associated with the "Alert" entry of telemetry.
        /// </summary>
        internal const string AlertTag = "Alert";

        /// <summary>
        /// The application logger.
        /// </summary>
        private readonly ILogger<AlertSink> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        public AlertSink(ILogger<AlertSink> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public void Emit(TelemetryEvent telemetryEvent)
        {
            _logger.LogError("{@metricEvent}", telemetryEvent);
        }

        /// <inheritdoc />
        /// <remarks>
        /// See the <see cref="LoggerSink"/> for non-alert telemetry.
        /// </remarks>
        public bool Predicate(TelemetryEvent telemetryEvent)
        {
            return telemetryEvent.Alert;
        }
    }
}