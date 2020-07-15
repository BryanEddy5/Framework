namespace HumanaEdge.Webcore.Framework.Web.Options
{
    /// <summary>
    /// Application configuration settings for the exception handling middleware.
    /// </summary>
    public class ExceptionHandlingOptions
    {
        /// <summary>
        /// Determines if the stack trace will be shown for the response when an exception is thrown.
        /// This is valuable for developers when debugging issues, but should not be used in production.
        /// </summary>
        public bool ShowExceptionDetails { get; set; }
    }
}