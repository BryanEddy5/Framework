using System;

namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Developer defined custom metric type.
    /// </summary>
    public sealed class CustomTelemetry : Telemetry
    {
        /// <summary>
        /// Designated constructor.
        /// </summary>
        /// <param name="name">Name of the observer metric.</param>
        /// <param name="configuration">Custom metrics defined for the class.</param>
        /// <param name="alert">Whether or not this telemetry contains an alert.</param>
        public CustomTelemetry(string name, TelemetryConfiguration configuration, bool alert)
            : base(name, TelemetryType.Custom, DateTimeOffset.UtcNow, configuration, alert)
        {
            // nop
        }
    }
}