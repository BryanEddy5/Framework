using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling.Converter
{
    /// <summary>
    /// Converts to exception messages.
    /// </summary>
    internal static class ExceptionMessageConverter
    {
        /// <summary>
        /// The payload key for adding the payload text to the exception message.
        /// </summary>
        internal const string PayloadKey = "payload";

        /// <summary>
        /// Converts to <see cref="ExceptionStorageMessage" /> from an <see cref="Exception" />.
        /// </summary>
        /// <remarks>
        /// Dynamically add the payload using a key to ensure the payload message is preserved and captured unaltered.
        /// </remarks>
        /// <param name="exception"> The thrown exception. </param>
        /// <param name="message"> The message payload. </param>
        /// <param name="applicationName"> The name of the application. </param>
        /// <returns> The storage message. </returns>
        public static Stream ToExceptionStorageMessage(this Exception exception, string message, string applicationName)
        {
            var exceptionMessage = new ExceptionStorageMessage(
                applicationName,
                exception.Message,
                exception.StackTrace,
                DateTimeOffset.UtcNow.ToString("o"),
                Activity.Current?.Id);

            var json = JsonConvert.SerializeObject(exceptionMessage);
            var jObject = JObject.Parse(json);

            var unescapedMessage = message.SanitizeMessage();
            jObject[PayloadKey] = unescapedMessage;
            return jObject.ToString().ToStream();
        }

        /// <summary>
        /// Removes the carriage returns it more readable.
        /// </summary>
        /// <param name="message"> The payload message. </param>
        /// <returns> The sanitized message. </returns>
        private static string SanitizeMessage(this string message) => message.Replace("\n", string.Empty);

        private static Stream ToStream(this string message) => new MemoryStream(Encoding.UTF8.GetBytes(message));
    }
}