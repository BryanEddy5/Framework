using System.Collections.Generic;

namespace HumanaEdge.Webcore.Core.Common.Pagination
{
    /// <summary>
    /// The paged list response contract.
    /// </summary>
    /// <typeparam name="T">The type of collection being returned.</typeparam>
    public class PagedListResponseModel<T>
    {
        /// <summary>
        /// The response data being returned.
        /// </summary>
        public IReadOnlyList<T> Data { get; set; } = null!;

        /// <summary>
        /// The pagination settings.
        /// </summary>
        public ResponsePageOptions Paging { get; set; } = null!;
    }
}