using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling
{
    /// <summary>
    /// The composition of the exception storage message.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    internal class ExceptionStorageMessage
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="timeStamp">A time stamp of when the exception occured.</param>
        /// <param name="traceParent">The trace parent from the W3C trace context.</param>
        public ExceptionStorageMessage(string applicationName, string errorMessage, string? stackTrace, string? timeStamp, string? traceParent)
        {
            ApplicationName = applicationName;
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
            TimeStamp = timeStamp;
            TraceParent = traceParent;
        }

        /// <summary>
        /// The name of the application.
        /// </summary>
        [JsonProperty("appName")]
        public string? ApplicationName { get; }

        /// <summary>
        /// The error message.
        /// </summary>
        [JsonProperty("errorMsg")]
        public string? ErrorMessage { get; }

        /// <summary>
        /// The stack trace.
        /// </summary>
        public string? StackTrace { get; }

        /// <summary>
        /// A time stamp of when the exception occured.
        /// </summary>
        [IgnoreDuringEquals]
        public string? TimeStamp { get; }

        /// <summary>
        /// The trace parent from the W3C trace context.
        /// </summary>
        public string? TraceParent { get; }
    }
}