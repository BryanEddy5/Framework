using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace HumanaEdge.Webcore.Core.Common.Exceptions
{
    /// <summary>
    /// An base http exception class that returns an http status code and detail for the consumer.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class MessageAppException : AppException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAppException" /> class.
        /// </summary>
        /// <param name="message">An error message associated with the exception.</param>
        /// <param name="exception">The inner exception.</param>
        public MessageAppException(string message, Exception? exception = null)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAppException" /> class.
        /// </summary>
        /// <param name="message">An error message associated with the exception.</param>
        /// <param name="exception">The inner exception.</param>
        /// <param name="loggedMessage">The  message to be logged. </param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public MessageAppException(Exception? exception, string message, string? loggedMessage = null, params object[]? args)
            : base(message, exception)
        {
            Args = args;
            LoggedMessage = loggedMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAppException" /> class.
        /// </summary>
        /// <param name="message">An error message associated with the exception.</param>
        /// <param name="loggedMessage">The  message to be logged. </param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public MessageAppException(string message, string? loggedMessage = null, params object[]? args)
            : base(message)
        {
            Args = args;
            LoggedMessage = loggedMessage;
        }

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public virtual HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;

        /// <summary>
        /// An object array that contains zero or more objects to format.
        /// </summary>
        public object[]? Args { get; }

        /// <summary>
        /// A message separate of the one being returned to the consumer.  This message is logged to give additional
        /// detail with structured logging about the exception thrown.
        /// </summary>
        public string? LoggedMessage { get; }
    }
}