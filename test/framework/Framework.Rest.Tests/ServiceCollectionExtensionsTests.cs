using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Framework.Rest.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    ///     Unit tests for <see cref="ServiceCollectionExtensions"/>.
    /// </summary>
    public class ServiceCollectionExtensionsTests
    {
        /// <summary>
        ///     Verifies the behavior of <see cref="ServiceCollectionExtensions.AddRestClient"/> method.
        /// </summary>
        [Fact]
        public void AddRestClient_InstantiateFactoryTest()
        {
            // arrange
            var provider = new ServiceCollection().AddRestClient().BuildServiceProvider();

            // act
            var actualRestClientFactory = provider.GetRequiredService<IRestClientFactory>();
            var actualMediaTypeFormatters =
                provider.GetServices<IMediaTypeFormatter>().FirstOrDefault(x => x.MediaType == MediaType.Json);

            // assert
            Assert.IsType<RestClientFactory>(actualRestClientFactory);
            Assert.IsType<JsonMediaTypeFormatter>(actualMediaTypeFormatters);
        }
    }
}