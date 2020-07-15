namespace HumanaEdge.Webcore.Core.Web
{
    /// <summary>
    /// Provides access to the request id.
    /// </summary>
    public interface IRequestIdAccessor
    {
        /// <summary>
        /// A unique request id for the current request.
        /// </summary>
        /// <returns>The request id.</returns>
        string CorrelationId { get; }

        /// <summary>
        /// The header key of the correlation id.
        /// </summary>
        public string Header { get; }
    }
}