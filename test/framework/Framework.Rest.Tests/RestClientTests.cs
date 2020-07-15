using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Core.Web;
using HumanaEdge.Webcore.Framework.Rest.Tests.Stubs;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    /// Unit tests for <see cref="RestClient" />.
    /// </summary>
    public class RestClientTests : BaseTests
    {
        private readonly string _fakeClientName;

        private readonly Mock<IInternalClient> _mockHttpClient;

        private readonly Mock<IInternalClientFactory> _mockInternalClientFactory;

        private readonly Mock<IMediaTypeFormatter> _mockMediaTypeFormatter;

        private readonly Mock<IRequestIdAccessor> _mockRequestIdAccessor;

        private readonly RestClientOptions _options;

        /// <summary>
        /// System under test.
        /// </summary>
        private RestClient _restClient;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RestClientTests()
        {
            _mockInternalClientFactory = Moq.Create<IInternalClientFactory>();
            _mockHttpClient = Moq.Create<IInternalClient>();
            _mockMediaTypeFormatter = Moq.Create<IMediaTypeFormatter>();
            _mockRequestIdAccessor = Moq.Create<IRequestIdAccessor>();

            _fakeClientName = FakeData.Create<string>();
            var fakeHeader = FakeData.Create<string>();
            var fakeCorrelationId = FakeData.Create<string>();

            _mockRequestIdAccessor.Setup(x => x.Header).Returns(fakeHeader);
            _mockRequestIdAccessor.Setup(x => x.CorrelationId).Returns(fakeCorrelationId);

            _options = new RestClientOptions.Builder("https://localhost:5000")
                .ConfigureHeader("Foo", "Bar")
                .ConfigureTimeout(TimeSpan.FromSeconds(7))
                .ConfigureJsonFormatting(
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
                .ConfigureMiddleware(
                    r =>
                    {
                        r.Headers["Id"] = Guid.NewGuid().ToString();
                        return r;
                    })
                .Build();

            _mockInternalClientFactory.Setup(x => x.CreateClient(_fakeClientName, _options.BaseUri, _options.Timeout))
                .Returns(_mockHttpClient.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync(RestRequest,CancellationToken)" /> with all <see cref="RestClientOptions"/> containing values.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendAsync_RestRequest_AllOptionsSet()
        {
            // arrange
            var fakeRequest = new RestRequest("/hello/world", HttpMethod.Get)
                .UseAcceptHeader(MediaType.Json)
                .UseHeader("test", "testing");
            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _mockHttpClient.Setup(
                    x => x.SendAsync(
                        It.Is<HttpRequestMessage>(
                            r =>
                                r.Method == fakeRequest.HttpMethod &&
                                r.Headers.Accept.Any(h => h.MediaType == MediaType.Json.MimeType) &&
                                r.Headers.GetValues("test").First() == "testing" &&
                                r.RequestUri == new Uri("/hello/world", UriKind.Relative) &&
                                r.Headers.Any(h => h.Key == "Id")),
                        CancellationTokenSource.Token))
                .ReturnsAsync(fakeHttpResponseMessage);

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _mockRequestIdAccessor.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object });

            // act
            var actual = await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestResponse>(actual);
            Assert.Empty(actual.ResponseBytes);
            Assert.Equal(fakeHttpResponseMessage.IsSuccessStatusCode, actual.IsSuccessful);
            Assert.Equal(fakeHttpResponseMessage.StatusCode, actual.StatusCode);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync{TRequest}(RestRequest{TRequest},CancellationToken)" />.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendAsync_RestRequest_WithRequestBody()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeRequest = new RestRequest<Foo>("/foo/add", HttpMethod.Post, fakeFoo, MediaType.Json);
            var mockContent = Moq.Create<HttpContent>();
            var fakeContent = mockContent.Object;
            _mockMediaTypeFormatter.Setup(x => x.TryFormat(MediaType.Json, _options, fakeFoo, out fakeContent))
                .Returns(true);
            _mockMediaTypeFormatter.Setup(x => x.MediaType).Returns(MediaType.Json);

            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            _mockHttpClient.Setup(
                    x => x.SendAsync(
                        It.Is<HttpRequestMessage>(
                            r =>
                                r.Method == fakeRequest.HttpMethod &&
                                r.RequestUri == new Uri("/foo/add", UriKind.Relative) &&
                                r.Content == fakeContent),
                        CancellationTokenSource.Token))
                .ReturnsAsync(fakeHttpResponseMessage);

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _mockRequestIdAccessor.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object });

            // act
            var actual = await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestResponse>(actual);
            Assert.Empty(actual.ResponseBytes);
            Assert.Equal(fakeHttpResponseMessage.IsSuccessStatusCode, actual.IsSuccessful);
            Assert.Equal(fakeHttpResponseMessage.StatusCode, actual.StatusCode);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync(RestRequest,CancellationToken)" />.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendAsync_RestRequest_WithResponseBody_Successful()
        {
            // arrange
            var fakeRequest = new RestRequest("/foo/add", HttpMethod.Get);
            var expected = FakeData.Create<Foo>();
            var json = $"{{\"Name\":\"{expected.Name}\", \"Age\":{expected.Age}}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var expectedBytes = await content.ReadAsByteArrayAsync();

            fakeHttpResponseMessage.Content = content;
            _mockHttpClient.Setup(
                    x => x.SendAsync(
                        It.Is<HttpRequestMessage>(
                            r =>
                                r.Method == fakeRequest.HttpMethod &&
                                r.RequestUri == new Uri("/foo/add", UriKind.Relative)),
                        CancellationTokenSource.Token))
                .ReturnsAsync(fakeHttpResponseMessage);

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _mockRequestIdAccessor.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object });

            // act
            var actual = await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestResponse>(actual);
            Assert.Equal(expectedBytes, actual.ResponseBytes);
            Assert.Equal(fakeHttpResponseMessage.IsSuccessStatusCode, actual.IsSuccessful);
            Assert.Equal(fakeHttpResponseMessage.StatusCode, actual.StatusCode);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync{TRequest}(RestRequest{TRequest},CancellationToken)" />.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendAsync_RestRequest_WithRequestBody_NotSupportedMediaType_Exception()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeRequest = new RestRequest<Foo>("/foo/add", HttpMethod.Post, fakeFoo, MediaType.Json);
            _mockMediaTypeFormatter.Setup(x => x.MediaType).Returns(null as MediaType);

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _mockRequestIdAccessor.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object });

            // act assert
            await Assert.ThrowsAsync<FormatFailedRestException>(async () => await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token));
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync{TRequest}(RestRequest{TRequest},CancellationToken)" />.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task SendAsync_RestRequest_WithRequestBody_RequestBodyTryFormat_Exception()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeRequest = new RestRequest<Foo>("/foo/add", HttpMethod.Post, fakeFoo, MediaType.Json);
            var mockContent = Moq.Create<HttpContent>();
            var fakeContent = mockContent.Object;
            _mockMediaTypeFormatter.Setup(x => x.MediaType).Returns(MediaType.Json);
            _mockMediaTypeFormatter.Setup(x => x.TryFormat(MediaType.Json, _options, fakeFoo, out fakeContent))
                .Returns(false);

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _mockRequestIdAccessor.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object });

            // act assert
            await Assert.ThrowsAsync<FormatFailedRestException>(async () => await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token));
        }
    }
}