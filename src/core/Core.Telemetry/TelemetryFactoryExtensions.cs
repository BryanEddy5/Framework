namespace HumanaEdge.Webcore.Core.Telemetry
{
    /// <summary>
    /// Extension methods on <see cref="ITelemetryFactory" />.
    /// </summary>
    public static class TelemetryFactoryExtensions
    {
        /// <summary>
        /// Factory creates an instance of CustomMetric.
        /// </summary>
        /// <param name="factory">The telemetry factory.</param>
        /// <param name="name">Name of the observer.</param>
        /// <param name="customTelemetry">Configuration data for the observer.</param>
        public static void TrackCustomTelemetry(
            this ITelemetryFactory factory,
            string name,
            TelemetryConfiguration customTelemetry)
        {
            var telemetryEvent = new CustomTelemetry(
                name,
                customTelemetry).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }
    }
}