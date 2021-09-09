namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Locations for the telemetry data to be transmitted to.
    /// </summary>
    public interface ITelemetrySink
    {
        /// <summary>
        /// Emits data to the designated sink location.
        /// </summary>
        /// <param name="telemetryEvent">A class of data to be emitted.</param>
        void Emit(TelemetryEvent telemetryEvent);

        /// <summary>
        /// Whether or not this sink should be used to track telemetry.
        /// </summary>
        /// <param name="telemetryEvent">The event to potentially track.</param>
        /// <returns>True if this sink should be leveraged, false otherwise.</returns>
        bool Predicate(TelemetryEvent telemetryEvent);
    }
}