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
        /// <param name="alert">Whether or not this telemetry contains an alert.</param>
        public static void TrackCustomTelemetry(
            this ITelemetryFactory factory,
            string name,
            TelemetryConfiguration customTelemetry,
            bool alert)
        {
            var telemetryEvent = new CustomTelemetry(
                name,
                customTelemetry,
                alert).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }
    }
}