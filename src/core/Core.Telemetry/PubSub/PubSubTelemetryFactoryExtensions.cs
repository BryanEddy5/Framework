using System;

namespace HumanaEdge.Webcore.Core.Telemetry.PubSub
{
    /// <summary>
    /// Extension methods for <see cref="ITelemetryFactory"/>.
    /// </summary>
    public static class PubSubTelemetryFactoryExtensions
    {
        /// <summary>
        /// Tracks a pub/sub subscription.
        /// </summary>
        /// <param name="factory">The telemetry factory.</param>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="messageId">The unique identifier for the pub/sub message.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        /// <param name="alert">Whether or not this telemetry contains an alert.</param>
        public static void TrackSubscriptionTelemetry(
            this ITelemetryFactory factory,
            DateTimeOffset startTime,
            string messageId,
            double duration,
            bool success = false,
            TelemetryConfiguration? configuration = null,
            bool alert = false)
        {
            var telemetryEvent = new PubSubTelemetry(
                "SubscriptionTelemetry",
                TelemetryType.Subscription,
                startTime,
                messageId,
                duration,
                success,
                configuration,
                alert).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }

        /// <summary>
        /// Tracks a pub/sub publication.
        /// </summary>
        /// <param name="factory">The telemetry factory.</param>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="messageId">The unique identifier for the pub/sub message.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        /// <param name="alert">Whether or not this telemetry contains an alert.</param>
        public static void TrackPublicationTelemetry(
            this ITelemetryFactory factory,
            DateTimeOffset startTime,
            string messageId,
            double duration,
            bool success = false,
            TelemetryConfiguration? configuration = null,
            bool alert = false)
        {
            var telemetryEvent = new PubSubTelemetry(
                "PublicationTelemetry",
                TelemetryType.Publication,
                startTime,
                messageId,
                duration,
                success,
                configuration,
                alert).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }
    }
}