using System.Net;

namespace HumanaEdge.Webcore.Core.Web
{
    /// <summary>
    /// A response with an explanation of the exception being thrown that is friendly for our consumer.
    /// </summary>
    public class ProblemDetail
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="title">The friendly message of the issue that is relayed.</param>
        /// <param name="requestId">A trace identifier for the request.</param>
        /// <param name="status">The returned result status code associated with the error.</param>
        /// <param name="message">A human-readable explanation specific to this occurrence of the problem.</param>
        /// <param name="traceId">The unique trace identifier that adheres to the W3C trace context.</param>
        public ProblemDetail(string title, string requestId, HttpStatusCode status, string message, string traceId = null!)
        {
            Title = title;
            RequestId = requestId;
            Status = status;
            Message = message;
            TraceId = traceId;
        }

        /// <summary>
        /// A human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The unique trace identifier that adheres to the W3C trace context.
        /// </summary>
        public string TraceId { get; }

        /// <summary>
        /// A trace identifier for the request.
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// The returned result status code associated with the error.
        /// </summary>
        public HttpStatusCode Status { get; }

        /// <summary>
        /// The friendly message of the issue that is relayed.
        /// </summary>
        public string Title { get; }
    }
}