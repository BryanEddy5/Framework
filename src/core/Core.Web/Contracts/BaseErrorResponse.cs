namespace HumanaEdge.Webcore.Core.Web.Contracts
{
    /// <summary>
    /// Common properties for an error response.
    /// </summary>
    public abstract class BaseErrorResponse
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="title">A message indicating the incoming request was bad.</param>
        /// <param name="traceId">The requests unique identifier.</param>
        /// <param name="status">The returned result status code associated with the error.</param>
        public BaseErrorResponse(string title, string traceId, int status)
        {
            Title = title;
            TraceId = traceId;
            Status = status;
        }

        /// <summary>
        /// A message indicating the incoming request was bad.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// The requests unique identifier.
        /// </summary>
        public string TraceId { get; }

        /// <summary>
        /// The returned result status code associated with the error.
        /// </summary>
        public int Status { get; }
    }
}