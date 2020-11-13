using System;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// A pub/sub specific exception that allows for Ack/Nack of the message.
    /// </summary>
    public class PubSubException : MessageAppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PubSubException" /> class.
        /// </summary>
        /// <param name="message">An error message associated with the exception.</param>
        /// <param name="exception">The inner exception.</param>
        public PubSubException(string message, Exception? exception = null)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Indicates if the exception is recoverable and the message should be Acked or Nacked.
        /// If the application can recover from the exception and the message should be retried then
        /// set to <see cref="Reply"/> to Nack.
        /// </summary>
        public virtual Reply Reply => Reply.Nack;
    }
}