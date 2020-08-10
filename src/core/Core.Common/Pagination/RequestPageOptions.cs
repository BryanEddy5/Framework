namespace HumanaEdge.Webcore.Core.Common.Pagination
{
    /// <summary>
    /// Pagination configuration settings https://gitlab.humanaedge.com/wiki/api-style#pagination.
    /// </summary>
    public class RequestPageOptions
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        public RequestPageOptions()
        {
            Limit = 10;
            Offset = 0;
        }

        /// <summary>
        /// The amount of records to be returned.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The number of records to skip before selecting records.
        /// </summary>
        public int Offset { get; set; }
    }
}