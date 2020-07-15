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
        /// <param name="statusCode">The returned result status code associated with the error.</param>
        /// <param name="detail">A human-readable explanation specific to this occurrence of the problem.</param>
        public ProblemDetail(string title, string requestId, HttpStatusCode statusCode, string detail)
        {
            Title = title;
            RequestId = requestId;
            StatusCode = statusCode;
            Detail = detail;
        }

        /// <summary>
        /// A human-readable explanation specific to this occurrence of the problem.
        /// </summary>
        public string Detail { get; }

        /// <summary>
        /// A trace identifier for the request.
        /// </summary>
        public string RequestId { get; }

        /// <summary>
        /// The returned result status code associated with the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The friendly message of the issue that is relayed.
        /// </summary>
        public string Title { get; }
    }
}