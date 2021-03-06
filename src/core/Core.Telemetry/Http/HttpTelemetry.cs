using System;
using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Telemetry.Http
{
    /// <summary>
    /// Http Telemetry for the duration of an Http request.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    internal abstract class HttpTelemetry : Telemetry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpTelemetry" /> class.
        /// </summary>
        /// <param name="name">Name of the observer.</param>
        /// <param name="type">The type of the telemetry.</param>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="resultCode">The response code of the request.</param>
        /// <param name="httpMethod">The HttpMethod used in the request.</param>
        /// <param name="uri">The Uri of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        /// <param name="alert">Whether or not this telemetry contains an alert.</param>
        internal HttpTelemetry(
            string name,
            TelemetryType type,
            DateTimeOffset startTime,
            double duration,
            string resultCode,
            string httpMethod,
            string uri,
            bool success,
            TelemetryConfiguration? configuration,
            bool alert)
            : base(name, type, startTime, configuration, alert)
        {
            Duration = duration;
            ResultCode = resultCode;
            Success = success;
            HttpMethod = httpMethod;
            Uri = uri;
        }

        /// <summary>
        /// Gets or sets the duration of the operation.
        /// </summary>
        public double Duration { get; }

        /// <summary>
        /// Gets or sets the duration of the operation.
        /// </summary>
        public string HttpMethod { get; }

        /// <summary>
        /// Gets or sets response code returned by the application after handling the request.
        /// </summary>
        public string ResultCode { get; }

        /// <summary>
        /// Get or Sets the success flag of the request.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Get or Sets the success flag of the request.
        /// </summary>
        public string Uri { get; }

        /// <inheritdoc />
        public override TelemetryEvent ToTelemetryEvent()
        {
            // shallow copy to a new dictionary to ensure this method is idempotent.
            var tags = new Dictionary<string, object>(Tags)
            {
                { nameof(Duration), Duration },
                { nameof(Success), Success },
                { nameof(HttpMethod), HttpMethod },
                { nameof(ResultCode), ResultCode },
                { nameof(Uri), Uri },
            };

            return new TelemetryEvent(
                Name,
                Type,
                Timestamp,
                tags,
                Alert);
        }
    }
}