using System;
using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Telemetry;

namespace HumanaEdge.Webcore.Framework.Telemetry
{
    /// <summary>
    /// Adds metrics to a registry.
    /// </summary>
    internal sealed class TelemetryFactory : ITelemetryFactory
    {
        /// <summary>
        /// Sink for data to be transmitted to for the metrics.
        /// </summary>
        private readonly ITelemetrySink[] _sinks;

        /// <summary>
        /// Designated Constructor.
        /// </summary>
        /// <param name="sinks">Sink for data to be transmitted to for the metrics.</param>
        public TelemetryFactory(IEnumerable<ITelemetrySink> sinks)
        {
            _sinks = sinks.ToArray() ?? throw new ArgumentNullException(nameof(sinks));
        }

        /// <inheritdoc />
        public void Track(TelemetryEvent telemetryEvent)
        {
            foreach (var sink in _sinks)
            {
                sink.Emit(telemetryEvent);
            }
        }
    }
}