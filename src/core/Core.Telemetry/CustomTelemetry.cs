using System;

namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Developer defined custom metric type.
    /// </summary>
    internal sealed class CustomTelemetry : Telemetry
    {
        /// <summary>
        /// Designated constructor.
        /// </summary>
        /// <param name="name">Name of the observer metric.</param>
        /// <param name="configuration">Custom metrics defined for the class.</param>
        internal CustomTelemetry(string name, TelemetryConfiguration configuration)
            : base(name, TelemetryType.Custom, DateTimeOffset.UtcNow, configuration)
        {
            // nop
        }
    }
}