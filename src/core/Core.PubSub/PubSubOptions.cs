namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// A config object that holds the required info to connect to a PubSub resource.
    /// </summary>
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
    }
}