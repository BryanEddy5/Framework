using HumanaEdge.Webcore.Core.Web.Contracts;

namespace HumanaEdge.Webcore.Core.Web
{
    /// <summary>
    /// A response with an explanation of the exception being thrown that is friendly for our consumer.
    /// </summary>
    public class ProblemDetail : BaseErrorResponse
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="title">The friendly message of the issue that is relayed.</param>
        /// <param name="requestId">A trace identifier for the request.</param>
        /// <param name="status">The returned result status code associated with the error.</param>
        /// <param name="message">A human-readable explanation specific to this occurrence of the problem.</param>
        /// <param name="traceId">The unique trace identifier that adheres to the W3C trace context.</param>
        public ProblemDetail(string title, string requestId, int status, string message, string traceId = null!)
            : base(title, traceId, status)
        {
            RequestId = requestId;
            Message = message;
        }

        /// <summary>
        /// A human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// A trace identifier for the request.
        /// </summary>
        public string RequestId { get; }
    }
}