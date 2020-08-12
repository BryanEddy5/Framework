using System.Collections.Generic;
using System.Linq;

namespace HumanaEdge.Webcore.Core.Common.Pagination
{
    /// <summary>
    /// Converters for <see cref="PagedListResponseModel{T}"/>.
    /// </summary>
    public static class PagedListConverter
    {
        /// <summary>
        /// Converts from a <see cref="IReadOnlyList{T}" /> to <see cref="PagedListResponseModel{T}" />.
        /// </summary>
        /// <param name="data">The collection to be converted.</param>
        /// <param name="requestPageOptions">The pagination configuration options.</param>
        /// <typeparam name="T">The response type.</typeparam>
        /// <returns><see cref="PagedListResponseModel{T}" />That is a response contract for our consumers.</returns>
        public static PagedListResponseModel<T> ToPagedListResponseModel<T>(
            this IReadOnlyList<T> data,
            RequestPageOptions requestPageOptions) =>
            new PagedListResponseModel<T>
            {
                Data = data,
                Paging = new ResponsePageOptions
                {
                    Offset = requestPageOptions.Offset,
                    Limit = requestPageOptions.Limit,
                    TotalCount = data.Count
                }
            };

        /// <summary>
        /// Converts from a <see cref="IReadOnlyList{T}" /> to <see cref="PagedListResponseModel{T}" /> while paginating the data
        /// in accordance with the <see cref="RequestPageOptions"/>.
        /// </summary>
        /// <param name="data">The collection to be converted.</param>
        /// <param name="requestPageOptions">The pagination configuration options.</param>
        /// <typeparam name="T">The response type.</typeparam>
        /// <returns><see cref="PagedListResponseModel{T}" />That is a response contract for our consumers.</returns>
        public static PagedListResponseModel<T> ToPagedList<T>(
            this IReadOnlyList<T> data,
            RequestPageOptions requestPageOptions) =>
            data.Skip(requestPageOptions.Offset)
                .Take(requestPageOptions.Limit)
                .ToArray()
                .ToPagedListResponseModel(requestPageOptions);
    }
}