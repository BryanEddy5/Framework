using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// This class packages the options for creating metrics into a single class (with subclasses per metric type)
    /// for easy extensibility of the API without adding numerous method overloads whenever new options are added.
    /// </summary>
    public class TelemetryConfiguration
    {
        /// <summary>
        /// Names of all the tag fields that are defined for the metric.
        /// </summary>
        public Dictionary<string, object> Tags { get; } = new Dictionary<string, object>();
    }
}