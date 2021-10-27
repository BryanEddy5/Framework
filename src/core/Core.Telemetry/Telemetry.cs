using System;
using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Base class for metrics, defining the basic informative API and the internal API.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public abstract class Telemetry
    {
        /// <summary>
        /// Designated constructor.
        /// </summary>
        /// <param name="name">Name of metric.</param>
        /// <param name="type">The type of the telemetry.</param>
        /// <param name="timestamp">The timestamp assigned to the telemetry event.</param>
        /// <param name="configuration">Containing tags and other metric data.</param>
        /// <param name="alert">Whether or not this telemetry contains an alert.</param>
        protected Telemetry(
            string name,
            TelemetryType type,
            DateTimeOffset timestamp,
            TelemetryConfiguration? configuration,
            bool alert = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            Timestamp = timestamp;
            Tags = configuration?.Tags ?? new Dictionary<string, object>();
            Alert = alert;
        }

        /// <summary>
        /// The Telemetry name, e.g. http_requests_total.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Names of the tags (name-value pairs) that apply to this Telemetry.
        /// This is an extra set of data points to be associated with the telemetry event.
        /// </summary>
        public Dictionary<string, object> Tags { get; }

        /// <summary>
        /// An occurence of the metric.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Indicator for the type of metric.
        /// </summary>
        public TelemetryType Type { get; }

        /// <summary>
        /// Whether or not this telemetry contains an alert.
        /// </summary>
        public bool Alert { get; }

        /// <summary>
        /// Write data to sink.
        /// </summary>
        /// <returns>Returns TelemetryEvent information.</returns>
        public virtual TelemetryEvent ToTelemetryEvent()
        {
            return new TelemetryEvent(
                Name,
                Type,
                Timestamp,
                Tags,
                Alert);
        }
    }
}