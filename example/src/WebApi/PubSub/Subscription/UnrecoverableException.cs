using System;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Subscription
{
    /// <summary>
    /// An example of an exception that occurs that is NOT recoverable, meaning that it should NOT be retried.
    /// </summary>
    public class UnrecoverableException : PubSubException
    {
        /// <summary>
        /// A constructor that intake a message.
        /// </summary>
        /// <param name="exception">An inner exception to be captured.</param>
        public UnrecoverableException(Exception? exception = null)
            : base($"A friendly message to indicate the exceptional event with some inputs", exception)
        {
        }

        /// <summary>
        /// Ack-ing the message will cause it to NOT be retried.
        /// </summary>
        public override Reply Reply => Reply.Ack;
    }
}