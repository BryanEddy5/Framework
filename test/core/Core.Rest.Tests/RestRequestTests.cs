using System.Linq;
using System.Net.Http;
using AutoFixture;
using HumanaEdge.Webcore.Core.Testing;
using Xunit;

namespace HumanaEdge.Webcore.Core.Rest.Tests
{
    /// <summary>
    ///     Unit tests for <see cref="RestRequest" />.
    /// </summary>
    public class RestRequestTests : BaseTests
    {
        /// <summary>
        ///     Validates the behavior of <see cref="RestRequest.UseAcceptHeader(MediaType)" />.
        /// </summary>
        [Fact]
        public void AddAcceptHeader_RestRequest()
        {
            // arrange
            var fakeMediaType = MediaType.Json;
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod)
                .UseAcceptHeader(fakeMediaType);
            var expected = new[] { fakeMediaType.MimeType };

            // act
            var actual = restRequest.Headers["Accept"];

            // assert
            Assert.Equal(expected, actual.ToArray());
        }

        /// <summary>
        ///     Validates the behavior of <see cref="RestRequest.UseAcceptHeader(MediaType)" />.
        /// </summary>
        [Fact]
        public void AddMultipleAcceptHeader_RestRequest()
        {
            // arrange
            var fakeMediaTypes = new[] { MediaType.Json, MediaType.Json };
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            foreach (var mediaType in fakeMediaTypes)
            {
                restRequest.UseAcceptHeader(mediaType);
            }

            var expectedMediaTypes = fakeMediaTypes.Select(x => x.MimeType).ToArray();

            // act
            var actual = restRequest.Headers["Accept"];

            // assert
            Assert.Equal(expectedMediaTypes, actual.ToArray());
        }

        /// <summary>
        ///     Validates the behavior of creating a <see cref="RestRequest" /> and setting the properties.
        /// </summary>
        [Fact]
        public void CreateNewRestRequest_ValidatePropertiesSet()
        {
            // arrange
            var expectedRelativePath = FakeData.Create<string>();
            var expectedHttpMethod = FakeData.Create<HttpMethod>();

            // act
            var actual = new RestRequest(expectedRelativePath, expectedHttpMethod);

            // assert
            Assert.Equal(expectedRelativePath, actual.RelativePath);
            Assert.Equal(expectedHttpMethod, actual.HttpMethod);
        }

        /// <summary>
        ///     Validates the behavior of <see cref="RestRequest.UseHeader(string, string)" />.
        /// </summary>
        [Fact]
        public void UseHeader_AddHeaderToRequest_Validation()
        {
            // arrange
            var fakeHeaderKey = FakeData.Create<string>();
            var expectedHeaderValues = FakeData.Create<string[]>();
            var fakeRelativePath = FakeData.Create<string>();
            var fakeHttpMethod = FakeData.Create<HttpMethod>();
            var restRequest = new RestRequest(fakeRelativePath, fakeHttpMethod);
            foreach (var header in expectedHeaderValues)
            {
                restRequest.UseHeader(fakeHeaderKey, header);
            }

            // act
            var actual = restRequest.Headers[fakeHeaderKey];

            // assert
            Assert.Equal(expectedHeaderValues, actual.ToArray());
        }
    }
}