using HumanaEdge.Webcore.Core.Storage;

namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// A config object that holds the required info to connect to a PubSub resource.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public class PubSubOptions
    {
        /// <summary>
        /// The subscriber has a configurable, limited amount of time -- known as the ackDeadline -- to acknowledge the outstanding
        /// message. Once the deadline passes, the message is no longer considered outstanding, and Pub/Sub will attempt to
        /// redeliver the message.
        /// Max of 600 seconds.
        /// </summary>
        public int AckDeadlineSeconds { get; set; } = 60;

        /// <summary>
        /// Duration before Ack Deadline which the message ACK deadline is automatically extended.
        /// Maximum of 3600 seconds.
        /// </summary>
        public int AckExtensionWindowSeconds { get; set; } = 15;

        /// <summary>
        /// The name of the resource in GCP.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The Id of the GCP Project in which the resource lives.
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// The maximum amount of messages that will be processed in parallel.
        /// </summary>
        public long? MaxMessageCount { get; set; } = 1;

        /// <summary>
        /// The maximum size of message payload that the application will process in bytes.
        /// </summary>
        public long? MaxMessageByteCount { get; set; }

        /// <summary>
        /// The resiliency configuration options for retrying a message.
        /// </summary>
        public ResiliencyOptions Resiliency { get; set; } = new ResiliencyOptions();

        /// <summary>
        /// The configuration settings for publishing to an exception storage bucket.
        /// </summary>
        public ExceptionStorageOptions ExceptionStorage { get; set; } = new ExceptionStorageOptions();

        /// <summary>
        /// The resiliency configuration options for retrying a message.
        /// </summary>
        [Equals(DoNotAddEqualityOperators = true)]
        public class ResiliencyOptions
        {
            /// <summary>
            /// The maximum number of retries before the message will be acked.
            /// </summary>
            public int MaxRetries { get; set; } = 10;

            /// <summary>
            /// The cache expiration for the retry cache key.
            /// </summary>
            public int RetryCacheExpirationInMinutes { get; set; } = 1440;
        }

        /// <summary>
        /// he configuration settings for publishing to an exception storage bucket.
        /// </summary>
        [Equals(DoNotAddEqualityOperators = true)]
        public class ExceptionStorageOptions : CloudStorageOptions
        {
            /// <summary>
            /// The name of the service/api that is subscribing to the topic.
            /// </summary>
            public string? ApplicationName { get; set; }
        }
    }
}