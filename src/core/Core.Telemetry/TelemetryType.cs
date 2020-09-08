namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// The indicator for the Metric Type.
    /// </summary>
    public enum TelemetryType
    {
        /// <summary>
        /// A custom metric.
        /// </summary>
        Custom,

        /// <summary>
        /// A request metric.
        /// </summary>
        Request,

        /// <summary>
        /// A dependency metric.
        /// </summary>
        Dependency
    }
}