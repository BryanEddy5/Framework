namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// Configuration settings for publishing a message.
    /// </summary>
    public class PublisherOptions
    {
        /// <summary>
        /// The project Id with the topic.
        /// </summary>
        public string? ProjectId { get; set; } = null!;

        /// <summary>
        /// The name of the topic to have the message published to.
        /// </summary>
        public string? TopicName { get; set; } = null!;
    }
}