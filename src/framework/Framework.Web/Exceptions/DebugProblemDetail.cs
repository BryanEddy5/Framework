using System;
using System.Net;
using HumanaEdge.Webcore.Core.Web;

namespace HumanaEdge.Webcore.Framework.Web.Exceptions
{
    /// <summary>
    /// Application response with debug information. Carries extra debug level information when allowed via configuration.
    /// </summary>
    [Serializable]
    public sealed class DebugProblemDetail : ProblemDetail
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="title">The friendly message of the issue that is relayed.</param>
        /// <param name="requestId">A trace identifier for the request.</param>
        /// <param name="status">The returned result status code associated with the error.</param>
        /// <param name="message">A human-readable explanation specific to this occurrence of the problem.</param>
        /// <param name="exception">The exception that the request failed with.</param>
        public DebugProblemDetail(
            string title,
            string requestId,
            HttpStatusCode status,
            string message,
            Exception exception)
            : base(title, requestId, status, message)
        {
            Exception = exception;
        }

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="problemDetail">The parent class.</param>
        /// <param name="exception">The exception that the request failed with.</param>
        public DebugProblemDetail(
            ProblemDetail problemDetail,
            Exception exception)
            : base(problemDetail.Title, problemDetail.RequestId, problemDetail.Status, problemDetail.Message)
        {
            Exception = exception;
        }

        /// <summary>
        /// Optional. The exception that the app failed with.
        /// </summary>
        public Exception Exception { get; }
    }
}