using Google.Cloud.PubSub.V1;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <summary>
    /// A service for generating a <see cref="PublishRequest"/>.
    /// </summary>
    internal interface IPublishRequestConverter
    {
        /// <summary>
        /// Creates a <see cref="PublishRequest"/> to be published to a topic.
        /// </summary>
        /// <param name="message">The payload of the message.</param>
        /// <param name="topicName">The identifier for the topic to be published to.</param>
        /// <typeparam name="TMessage">The shape of the payload.</typeparam>
        /// <returns>A publish request to be sent.</returns>
        PublishRequest Create<TMessage>(TMessage message, TopicName topicName);
    }
}