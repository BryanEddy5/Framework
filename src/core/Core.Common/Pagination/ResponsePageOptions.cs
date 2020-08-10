namespace HumanaEdge.Webcore.Core.Common.Pagination
{
    /// <summary>
    /// Pagination configuration settings https://gitlab.humanaedge.com/wiki/api-style#pagination.
    /// </summary>
    public sealed class ResponsePageOptions : RequestPageOptions
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        public ResponsePageOptions()
        {
            TotalCount = 0;
        }

        /// <summary>
        /// The total count of the documents.
        /// </summary>
        public int TotalCount { get; set; }
    }
}