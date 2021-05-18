using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Core.Web.Contracts;
using HumanaEdge.Webcore.Example.WebApi;
using HumanaEdge.Webcore.Example.WebApi.Controllers;
using HumanaEdge.Webcore.Example.WebApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Web.Tests
{
    /// <summary>
    /// Unit test to ensure a bad request properly deserializes to <see cref="BadRequestResponse"/>.
    /// </summary>
    public class BadRequestDeserialization : BaseTests, IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="factory">Creates client for testing.</param>
        public BadRequestDeserialization(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        /// <summary>
        /// Verifies the deserialization of <see cref="BadRequestResponse"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task DeserializeBadRequest()
        {
            // arrange
            var fakeFoo = FakeData.Build<Foo>()
                .Without(x => x.Bar)
                .Create();
            var json = JsonConvert.SerializeObject(fakeFoo, StandardSerializerConfiguration.Settings);
            var content = new StringContent(json, Encoding.UTF8, MediaType.Json.MimeType);

            // act
            var actual = await _httpClient.PostAsync(BadRequestController.Route, content);

            // assert
            var payload = await actual.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BadRequestResponse>(
                payload,
                StandardSerializerConfiguration.Settings);
            result.Should().NotBeNull();
            result.Status.Should().Be(400);
            result.Errors.Should().NotBeEmpty();
            result.Errors.Keys.Should().Contain("Bar");
        }
    }
}