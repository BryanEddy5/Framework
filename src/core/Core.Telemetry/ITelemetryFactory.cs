namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Factory pattern for creating metrics.
    /// </summary>
    public interface ITelemetryFactory
    {
        /// <summary>
        /// Tracks a single telemetry event.
        /// </summary>
        /// <param name="telemetryEvent">The event to be tracked.</param>
        void Track(TelemetryEvent telemetryEvent);
    }
}