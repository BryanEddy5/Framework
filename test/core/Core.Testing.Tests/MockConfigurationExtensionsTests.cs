using System.Collections.Generic;
using System.Linq;
using HumanaEdge.Webcore.Core.Common;
using HumanaEdge.Webcore.Core.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HumanaEdge.Webcore.Coe.Testing.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="MockConfigurationExtensions" /> class.
    /// </summary>
    public class MockConfigurationExtensionsTests : BaseTests
    {
        /// <summary>
        /// Verifies the behavior of the <see cref="MockConfigurationExtensions.SetupGetList" /> method.
        /// </summary>
        [Fact]
        public void SetupGetListTest()
        {
            // arrange
            var fakeKey = "Fake:Key";
            var fakeList = new List<string>
            {
                "hello",
                "world"
            };
            var mockConfiguration = Moq.Create<IConfiguration>();
            mockConfiguration.SetupGetList(fakeKey).ReturnsList(fakeList);

            // act
            var actualList = mockConfiguration.Object.GetList(fakeKey);

            // assert
            Assert.Equal(fakeList, actualList.ToList());
        }
    }
}