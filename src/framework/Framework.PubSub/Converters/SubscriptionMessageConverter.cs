using System;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.PubSub;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.PubSub.Converters
{
    /// <summary>
    /// A converter class for converting data transfer objects.
    /// </summary>
    public static class SubscriptionMessageConverter
    {
        /// <summary>
        /// Converts <see cref="PubsubMessage"/> to <see cref="SubscriptionMessage{TMessage}"/>.
        /// </summary>
        /// <param name="message">The gcp pub sub message.</param>
        /// <typeparam name="TMessage">The message shape.</typeparam>
        /// <returns>The subscription message.</returns>
        public static SubscriptionMessage<TMessage> ToSubscriptionMessage<TMessage>(this PubsubMessage message)
        {
            var deserializedMessage = new Lazy<TMessage>(
                () => JsonConvert.DeserializeObject<TMessage>(
                    message.Data.ToStringUtf8(),
                    StandardSerializerConfiguration.Settings)!);
            return new SubscriptionMessage<TMessage>(
                message.Data.ToByteArray(),
                deserializedMessage,
                message.MessageId);
        }
    }
}