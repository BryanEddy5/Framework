using System;

namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// The subscription message.
    /// </summary>
    /// <typeparam name="TMessage">The shape of the message.</typeparam>
    [Equals(DoNotAddEqualityOperators = true)]
    public class SubscriptionMessage<TMessage>
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="payload">The payload of the message.</param>
        /// <param name="message">The deserialized message.</param>
        /// <param name="messageId">The unique identifier for the message in that subscription. </param>
        public SubscriptionMessage(byte[] payload, Lazy<TMessage> message, string messageId)
        {
            Payload = payload;
            Message = message;
            MessageId = messageId;
        }

        /// <summary>
        /// The payload of the message.
        /// </summary>
        public byte[] Payload { get; }

        /// <summary>
        /// The deserialized message.
        /// </summary>
        [IgnoreDuringEquals]
        public Lazy<TMessage> Message { get; }

        /// <summary>
        /// The unique identifier for the message in that subscription.
        /// </summary>
        public string MessageId { get; }
    }
}