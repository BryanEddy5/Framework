namespace HumanaEdge.Webcore.Framework.PubSub.TraceContext
{
    /// <summary>
    /// Centralized W3C trace context headers.
    /// </summary>
    public class TracingKeys
    {
        /// <summary>
        /// The trace id from for tracing a request as it crosses application boundaries.
        /// </summary>
        public const string TraceId = "TraceId";

        /// <summary>
        /// The property key for a message id when creating log context.
        /// </summary>
        public const string MessageId = "MessageId";

        /// <summary>
        /// Shows each unique Request Id from each service delimited by a pipe (|).
        /// </summary>
        public const string SpanId = "SpanId";

        /// <summary>
        /// The request id from the the service that sent the message.
        /// </summary>
        public const string ParentId = "ParentId";

        /// <summary>
        /// A unique identifier for the request.
        /// </summary>
        public const string RequestId = "RequestId";
    }
}