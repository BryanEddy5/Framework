using System;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;

namespace HumanaEdge.Webcore.Example.WebApi.PubSub.Subscription
{
    /// <summary>
    /// An example of an exception that occurs that is recoverable, meaning that it should be retried.
    /// </summary>
    /// <remarks>A good use case would be a 500 thrown during an http integration or another ephemeral event that could clear up at a later time.</remarks>
    public class RecoverableException : PubSubException
    {
        /// <summary>
        /// A constructor that intake a message.
        /// </summary>
        /// <param name="exception">An inner exception to be captured.</param>
        public RecoverableException(Exception? exception = null)
            : base($"A friendly message to indicate the exceptional event with some inputs", exception)
        {
        }

        /// <summary>
        /// Nack-ing the message will cause it to be retried in accordance with the Subscription settings in GCP.
        /// </summary>
        public override Reply Reply => Reply.Nack;
    }
}