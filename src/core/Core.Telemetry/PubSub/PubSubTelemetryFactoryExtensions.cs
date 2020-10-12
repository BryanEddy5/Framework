using System;

namespace HumanaEdge.Webcore.Core.Telemetry.PubSub
{
    /// <summary>
    /// Extension methods for <see cref="ITelemetryFactory"/>.
    /// </summary>
    public static class PubSubTelemetryFactoryExtensions
    {
        /// <summary>
        /// Tracks an HTTP dependency.
        /// </summary>
        /// <param name="factory">The telemetry factory.</param>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="messageId">The unique identifier for the pub/sub message.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        public static void TrackSubscriptionTelemetry(
            this ITelemetryFactory factory,
            DateTimeOffset startTime,
            string messageId,
            double duration,
            bool success = false,
            TelemetryConfiguration? configuration = null)
        {
            var telemetryEvent = new SubscriptionTelemetry(
                startTime,
                messageId,
                duration,
                success,
                configuration).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }
    }
}