using System;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs
{
    /// <summary>
    /// Exception with reply of Ack for testing.
    /// </summary>
    public class AckException : PubSubException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AckException" /> class.
        /// </summary>
        /// <param name="message">An error message associated with the exception.</param>
        /// <param name="exception">The inner exception.</param>
        public AckException(string message, Exception exception = null)
            : base(message, exception)
        {
        }

        /// <inheritdoc />
        public override Reply Reply => Reply.Ack;
    }
}