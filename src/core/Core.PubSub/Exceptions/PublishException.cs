using System;
using System.Net;
using HumanaEdge.Webcore.Core.Common.Exceptions;

namespace HumanaEdge.Webcore.Core.PubSub.Exceptions
{
    /// <summary>
    /// An exception thrown during a publication event.
    /// </summary>
    public class PublishException : MessageAppException
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="message">Information about the exception thrown.</param>
        /// <param name="exception">An inner exception to be captured.</param>
        public PublishException(string message, Exception exception = null!)
            : base(message, exception)
        {
        }

        /// <inheritdoc />
        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.ServiceUnavailable;
    }
}