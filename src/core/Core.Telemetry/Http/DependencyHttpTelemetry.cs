using System;

namespace HumanaEdge.Webcore.Core.Telemetry.Http
{
    /// <summary>
    /// Dependency Telemetry.
    /// </summary>
    internal sealed class DependencyHttpTelemetry : HttpTelemetry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyHttpTelemetry" /> class.
        /// </summary>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="resultCode">The response code of the request.</param>
        /// <param name="dependencyHttpMethod">The Dependency HttpMethod used in the request.</param>
        /// <param name="dependencyUri">The Dependency Uir of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        internal DependencyHttpTelemetry(
            DateTimeOffset startTime,
            double duration,
            string resultCode,
            string dependencyHttpMethod,
            string dependencyUri,
            bool success,
            TelemetryConfiguration? configuration)
            : base(
                "HttpDependencyTelemetry",
                TelemetryType.Dependency,
                startTime,
                duration,
                resultCode,
                dependencyHttpMethod,
                dependencyUri,
                success,
                configuration)
        {
            // nop
        }
    }
}