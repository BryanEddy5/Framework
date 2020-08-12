using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using HumanaEdge.Webcore.Core.Common.Pagination;
using HumanaEdge.Webcore.Core.Testing;
using Xunit;

namespace HumanaEdge.Webcore.Core.Common.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="PagedListConverter" /> extension
    /// methods.
    /// </summary>
    public class PaginationTests : BaseTests
    {
        /// <summary>
        /// Validates the behavior of
        /// <see cref="PagedListConverter.ToPagedList{T}" />
        /// returns the correct response data.
        /// </summary>
        [Fact]
        public void List_ToPaginatedList_ReturnsExpectedDataSet()
        {
            // arrange
            var list = GetSampleList();
            var pageOptions = new RequestPageOptions();
            pageOptions.Limit = 5;
            pageOptions.Offset = 3;

            // act
            var pagedList = list.ToPagedList(pageOptions);

            // assert
            pagedList.Data.Should()
                .HaveCount(5)
                .And.ContainInOrder("Delta", "Echo", "Foxtrot", "Golf", "Hotel");
        }

        /// <summary>
        /// Validates the behavior of
        /// <see cref="PagedListConverter.ToPagedListResponseModel{T}" />
        /// returns the correct response data.
        /// </summary>
        [Fact]
        public void List_ToPagedList_ReturnsExpectedDataSet()
        {
            // arrange
            var list = GetSampleList();
            var pageOptions = new RequestPageOptions();

            // act
            var pagedList = list.ToPagedListResponseModel(pageOptions);

            // assert
            pagedList.Data.Should()
                .HaveCount(GetSampleList().Count)
                .And.ContainInOrder(GetSampleList());
        }

        /// <summary>
        /// Validates the behavior of
        /// <see cref="PagedListConverter.ToPagedListResponseModel{T}" />
        /// return the correct count.
        /// </summary>
        [Fact]
        public void PagedList_Constructor_ReturnsCorrectCount()
        {
            // arrange
            var list = GetSampleList();
            var pageOptions = new RequestPageOptions();

            // act
            var newList = list.Skip(pageOptions.Offset)
                .Take(pageOptions.Limit)
                .ToArray();
            var pagedList = newList.ToPagedListResponseModel(pageOptions);

            // assert
            pagedList.Data.Should().HaveCount(10);
            pagedList.Paging.Limit.IsSameOrEqualTo(10);
            pagedList.Paging.Offset.IsSameOrEqualTo(0);
        }

        /// <summary>
        /// Returns a list of sample data.
        /// </summary>
        /// <returns>Sample data.</returns>
        private IReadOnlyList<string> GetSampleList()
        {
            // arrange
            var list = new List<string>();
            list.Add("Alpha");
            list.Add("Bravo");
            list.Add("Charlie");
            list.Add("Delta");
            list.Add("Echo");
            list.Add("Foxtrot");
            list.Add("Golf");
            list.Add("Hotel");
            list.Add("India");
            list.Add("Juliet");
            list.Add("Kilo");
            list.Add("Lima");
            list.Add("Mike");
            list.Add("November");
            return list;
        }
    }
}