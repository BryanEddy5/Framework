using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing;
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
        private readonly Mock<IInternalClient> _mockHttpClient;

        private readonly Mock<IMediaTypeFormatter> _mockMediaTypeFormatter;

        private readonly RestClientOptions _options;

        /// <summary>
        /// System under test.
        /// </summary>
        private readonly RestClient _restClient;

        private Mock<IInternalClientFactory> _mockInternalClientFactory;

        private string _fakeClientName;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RestClientTests()
        {
            _mockInternalClientFactory = Moq.Create<IInternalClientFactory>();
            _mockHttpClient = Moq.Create<IInternalClient>();
            _mockMediaTypeFormatter = Moq.Create<IMediaTypeFormatter>();
            _mockMediaTypeFormatter.Setup(f => f.MediaTypes).Returns(new[] { MediaType.Json });
            _fakeClientName = FakeData.Create<string>();

            _options = new RestClientOptions.Builder("https://localhost:5000")
                .ConfigureHeader("Foo", "Bar")
                .ConfigureTimeout(TimeSpan.FromSeconds(7))
                .ConfigureJsonFormatting(
                    StandardSerializerConfiguration.Settings)
                .ConfigureMiddleware(
                    r =>
                    {
                        r.Headers["Id"] = Guid.NewGuid().ToString();
                        return r;
                    })
                .Build();

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object });
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync(RestRequest,CancellationToken)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
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
            SetUpHttpClientMock(
                fakeHttpResponseMessage,
                r =>
                    r.Method == fakeRequest.HttpMethod &&
                    r.Headers.Accept.Any(h => h.MediaType == MediaType.Json.MimeType) &&
                    r.Headers.GetValues("test").First() == "testing" &&
                    r.RequestUri == new Uri("/hello/world", UriKind.Relative) &&
                    r.Headers.Any(h => h.Key == "Id"));
            SetupHttClientFactory();

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

            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            SetUpHttpClientMock(
                fakeHttpResponseMessage,
                r =>
                    r.Method == fakeRequest.HttpMethod &&
                    r.RequestUri == new Uri("/foo/add", UriKind.Relative) &&
                    r.Content == fakeContent);
            SetupHttClientFactory();

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
        public async Task SendAsync_RestRequest_WithRequestBody_NotSupportedMediaType_Exception()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeRequest = new RestRequest<Foo>("/foo/add", HttpMethod.Post, fakeFoo, MediaType.Json);

            // act assert
            await Assert.ThrowsAsync<FormatFailedRestException>(
                async () => await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token));
        }

        /// <summary>
        /// Testing the FileResponse is populated validly.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetFileAsync_Okay()
        {
            // arrange
            var fakeRequest = new RestRequest("/foo/file", HttpMethod.Get);
            var expectedBytes = FakeData.Create<byte[]>();
            var expectedBytesStream = new MemoryStream(expectedBytes);
            var mockResponse = BuildMockResponseForFiles(expectedBytesStream);
            SetupHttClientFactory();
            SetUpHttpClientMock(
                mockResponse,
                r =>
                    r.Method == fakeRequest.HttpMethod &&
                    r.RequestUri == new Uri("/foo/file", UriKind.Relative));

            // act
            var actual = await _restClient.GetFileAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            actual.Should().BeOfType<FileResponse>();
            actual.FileStream.Length.Should().Be(expectedBytesStream.Length);
            actual.IsSuccessful.Should().Be(mockResponse.IsSuccessStatusCode);
            actual.StatusCode.Should().Be(mockResponse.StatusCode);
            actual.ContentType.Should().Be(mockResponse.Content.Headers.ContentType.MediaType);
            actual.FileName.Should().Be(mockResponse.Content.Headers.ContentDisposition.FileName);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync{TRequest}(RestRequest{TRequest},CancellationToken)" />.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task GetFileAsync_RestRequest_WithRequestBody()
        {
            // arrange
            var fakeFoo = FakeData.Create<Foo>();
            var fakeRequest = new RestRequest<Foo>("/foo/file", HttpMethod.Post, fakeFoo, MediaType.Json);
            var mockContent = Moq.Create<HttpContent>();
            var fakeContent = mockContent.Object;
            _mockMediaTypeFormatter.Setup(x => x.TryFormat(MediaType.Json, _options, fakeFoo, out fakeContent))
                .Returns(true);
            SetupHttClientFactory();

            var expectedBytes = FakeData.Create<byte[]>();
            var expectedBytesStream = new MemoryStream(expectedBytes);
            var mockResponse = BuildMockResponseForFiles(expectedBytesStream);
            SetUpHttpClientMock(
                mockResponse,
                r =>
                    r.Method == fakeRequest.HttpMethod &&
                    r.RequestUri == new Uri("/foo/file", UriKind.Relative) &&
                    r.Content == fakeContent);

            // act
            var actual = await _restClient.GetFileAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            actual.Should().BeOfType<FileResponse>();
            actual.FileStream.Length.Should().Be(expectedBytesStream.Length);
            actual.IsSuccessful.Should().Be(mockResponse.IsSuccessStatusCode);
            actual.StatusCode.Should().Be(mockResponse.StatusCode);
            actual.ContentType.Should().Be(mockResponse.Content.Headers.ContentType.MediaType);
            actual.FileName.Should().Be(mockResponse.Content.Headers.ContentDisposition.FileName);
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
            _mockMediaTypeFormatter.Setup(x => x.TryFormat(MediaType.Json, _options, fakeFoo, out fakeContent))
                .Returns(false);

            // act assert
            await Assert.ThrowsAsync<FormatFailedRestException>(
                async () => await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token));
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
            SetupHttClientFactory();

            fakeHttpResponseMessage.Content = content;
            _mockHttpClient.Setup(
                    x => x.SendAsync(
                        It.Is<HttpRequestMessage>(
                            r =>
                                r.Method == fakeRequest.HttpMethod &&
                                r.RequestUri == new Uri("/foo/add", UriKind.Relative)),
                        CancellationTokenSource.Token))
                .ReturnsAsync(fakeHttpResponseMessage);

            // act
            var actual = await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestResponse>(actual);
            Assert.Equal(expectedBytes, actual.ResponseBytes);
            Assert.Equal(fakeHttpResponseMessage.IsSuccessStatusCode, actual.IsSuccessful);
            Assert.Equal(fakeHttpResponseMessage.StatusCode, actual.StatusCode);
        }

        /// <summary>
        /// Verifies that no exception is thrown when the file name returns as null.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task GetFileAsync_NullContentDisposition_ShouldNotThrowException()
        {
            // arrange
            var fakeRequest = new RestRequest("/foo/file", HttpMethod.Get);
            var expectedBytes = FakeData.Create<byte[]>();
            var expectedBytesStream = new MemoryStream(expectedBytes);
            var content = new StreamContent(expectedBytesStream);
            SetupHttClientFactory();
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockResponse.Content = content;
            mockResponse.Content.Headers.ContentDisposition = null;
            mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            SetUpHttpClientMock(
                mockResponse,
                r =>
                    r.Method == fakeRequest.HttpMethod &&
                    r.RequestUri == new Uri("/foo/file", UriKind.Relative));

            // act
            var actual = await _restClient.GetFileAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            actual.FileName.Should().Be(null);
        }

        private HttpResponseMessage BuildMockResponseForFiles(MemoryStream expectedBytesStream)
        {
            var content = new StreamContent(expectedBytesStream);

            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockResponse.Content = content;
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.FileName = "SomeFile.txt";
            mockResponse.Content.Headers.ContentDisposition = contentDisposition;
            mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return mockResponse;
        }

        private void SetUpHttpClientMock(
            HttpResponseMessage fakeHttpResponseMessage,
            Expression<Func<HttpRequestMessage, bool>> match) =>
            _mockHttpClient.Setup(
                    x => x.SendAsync(It.Is(match), CancellationTokenSource.Token))
                .ReturnsAsync(fakeHttpResponseMessage);

        private void SetupHttClientFactory() =>
            _mockInternalClientFactory.Setup(
                x => x.CreateClient(_fakeClientName, _options.BaseUri, _options.Timeout)).Returns(_mockHttpClient.Object);
    }
}