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
    }
}