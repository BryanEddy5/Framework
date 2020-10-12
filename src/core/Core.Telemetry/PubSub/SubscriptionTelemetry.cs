using System;
using System.Collections.Generic;
using System.Net.Http;

namespace HumanaEdge.Webcore.Core.Telemetry.PubSub
{
    /// <summary>
    /// Telemetry tailored for subscriptions.
    /// </summary>
    internal class SubscriptionTelemetry : Telemetry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionTelemetry"/> class.
        /// </summary>
        /// <param name="startTime">Start of the request.</param>
        /// <param name="messageId">The unique identifier for the pub/sub message.</param>
        /// <param name="duration">The duration of the request.</param>
        /// <param name="success">Indicator if the request was successful.</param>
        /// <param name="configuration">Configuration data for the observer.</param>
        public SubscriptionTelemetry(
            DateTimeOffset startTime,
            string messageId,
            double duration,
            bool success,
            TelemetryConfiguration? configuration)
            : base(nameof(SubscriptionTelemetry), TelemetryType.Subscription, startTime, configuration)
        {
            Success = success;
            MessageId = messageId;
            Duration = duration;
        }

        /// <summary>
        /// Get or Sets the success flag of the request.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets or sets the duration of the operation.
        /// </summary>
        public double Duration { get; }

        /// <summary>
        /// The unique identifier for the pub/sub message.
        /// </summary>
        public string MessageId { get; }

        /// <inheritdoc />
        internal override TelemetryEvent ToTelemetryEvent()
        {
            // shallow copy to a new dictionary to ensure this method is idempotent.
            var tags = new Dictionary<string, object>(Tags);
            tags.Add(nameof(Duration), Duration);
            tags.Add(nameof(Success), Success);
            tags.Add(nameof(MessageId), MessageId);

            var telemetry = new TelemetryEvent(
                Name,
                Type,
                Timestamp,
                tags);

            return telemetry;
        }
    }
}