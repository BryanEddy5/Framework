using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using HumanaEdge.Webcore.Core.Common;

namespace HumanaEdge.Webcore.Core.Web
{
    /// <summary>
    ///     An base http exception class that returns an http status code and detail for the consumer.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MessageAppException : AppException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageAppException" /> class.
        /// </summary>
        /// <param name="message">An error message associated with the exception.</param>
        /// <param name="exception">The inner exception.</param>
        public MessageAppException(string message, Exception? exception = null)
            : base(message, exception)
        {
        }

        /// <summary>
        ///     HTTP status code.
        /// </summary>
        public virtual HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
    }
}