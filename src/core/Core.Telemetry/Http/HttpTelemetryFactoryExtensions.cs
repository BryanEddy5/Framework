using System;

namespace HumanaEdge.Webcore.Core.Telemetry.Http
{
    /// <summary>
    /// Extension methods on <see cref="ITelemetryFactory" /> specialized for tracking HTTP requests.
    /// </summary>
    public static class HttpTelemetryFactoryExtensions
    {
        /// <summary>
        /// Tracks an HTTP dependency.
        /// </summary>
        /// <param name="factory">The telemetry factory.</param>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="responseCode">The response code of the request.</param>
        /// <param name="httpMethod">The HttpMethod used in the request.</param>
        /// <param name="uri">The Uri of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        public static void TrackDependencyHttpTelemetry(
            this ITelemetryFactory factory,
            DateTimeOffset startTime,
            double duration,
            string responseCode,
            string httpMethod,
            string uri,
            bool success = false,
            TelemetryConfiguration? configuration = null)
        {
            var telemetryEvent = new DependencyHttpTelemetry(
                startTime,
                duration,
                responseCode,
                httpMethod,
                uri,
                success,
                configuration).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }

        /// <summary>
        /// Tracks an HTTP request.
        /// </summary>
        /// <param name="factory">The telemetry factory.</param>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="responseCode">The response code of the request.</param>
        /// <param name="httpMethod">The HttpMethod used in the request.</param>
        /// <param name="uri">The Uri of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        public static void TrackRequestHttpTelemetry(
            this ITelemetryFactory factory,
            DateTimeOffset startTime,
            double duration,
            string responseCode,
            string httpMethod,
            string uri,
            bool success = false,
            TelemetryConfiguration? configuration = null)
        {
            var telemetryEvent = new RequestHttpTelemetry(
                startTime,
                duration,
                responseCode,
                httpMethod,
                uri,
                success,
                configuration).ToTelemetryEvent();
            factory.Track(telemetryEvent);
        }
    }
}