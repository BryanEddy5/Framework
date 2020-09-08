using System;

namespace HumanaEdge.Webcore.Core.Telemetry.Http
{
    /// <summary>
    /// Telemetry for an HTTP request.
    /// </summary>
    internal sealed class RequestHttpTelemetry : HttpTelemetry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHttpTelemetry" /> class.
        /// </summary>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="resultCode">The response code of the request.</param>
        /// <param name="httpMethod">The Dependency HttpMethod used in the request.</param>
        /// <param name="uri">The Dependency Uir of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Additional Metadata about the request.</param>
        internal RequestHttpTelemetry(
            DateTimeOffset startTime,
            double duration,
            string resultCode,
            string httpMethod,
            string uri,
            bool success,
            TelemetryConfiguration? configuration)
            : base(
                "HttpRequestTelemetry",
                TelemetryType.Request,
                startTime,
                duration,
                resultCode,
                httpMethod,
                uri,
                success,
                configuration)
        {
            // nop
        }
    }
}