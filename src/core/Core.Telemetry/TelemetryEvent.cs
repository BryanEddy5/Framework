using System;
using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Captures the data from a Metric.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public sealed class TelemetryEvent
    {
        /// <summary>
        /// Designated constructor.
        /// </summary>
        /// <param name="name">Name of the metric.</param>
        /// <param name="telemetryType">The indicator of the metric type.</param>
        /// <param name="timestamp">The time at which the metric was captured.</param>
        /// <param name="tags">Names of the tags (name-value pairs) that apply to this metric.</param>
        /// <param name="alert">Whether or not this telemetry has an alert.</param>
        public TelemetryEvent(
            string name,
            TelemetryType telemetryType,
            DateTimeOffset timestamp,
            IReadOnlyDictionary<string, object> tags,
            bool alert)
        {
            Name = name;
            TelemetryType = telemetryType;
            Timestamp = timestamp;
            Tags = tags;
            Alert = alert;
        }

        /// <summary>
        /// Name of the Metric.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Names of the tags (name-value pairs) that apply to this Metric.
        /// </summary>
        public IReadOnlyDictionary<string, object> Tags { get; }

        /// <summary>
        /// Indicator for the metric type that was captured.
        /// </summary>
        public TelemetryType TelemetryType { get; }

        /// <summary>
        /// The time at which the event was captured.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Whether or not this telemetry has an alert.
        /// </summary>
        public bool Alert { get; }
    }
}