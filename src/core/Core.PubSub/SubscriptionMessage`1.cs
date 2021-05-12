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
        public SubscriptionMessage(byte[] payload, Lazy<TMessage> message)
        {
            Payload = payload;
            Message = message;
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
    }
}