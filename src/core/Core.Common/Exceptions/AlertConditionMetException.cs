using System.Net;

namespace HumanaEdge.Webcore.Core.Common.Exceptions
{
    /// <summary>
    /// Thrown if an alert condition is met and the ThrowOnFailure flag is toggled.
    /// </summary>
    public class AlertConditionMetException : MessageAppException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exception">The inner exception, if available.</param>
        public AlertConditionMetException(MessageAppException? exception = null)
            : base("An alert condition was met.", exception)
        {
            if (exception != null)
            {
                StatusCode = exception.StatusCode;
            }
        }

        /// <summary>
        /// The overriden status code of this exception.
        /// </summary>
        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.InternalServerError;
    }
}