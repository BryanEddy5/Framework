﻿using System;
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
using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Resiliency;
using HumanaEdge.Webcore.Framework.Rest.Tests.Stubs;
using Moq;
using Polly;
using Xunit;
using RestRequest = HumanaEdge.Webcore.Core.Rest.RestRequest;
using RestResponse = HumanaEdge.Webcore.Core.Rest.RestResponse;

namespace HumanaEdge.Webcore.Framework.Rest.Tests
{
    /// <summary>
    /// Unit tests for <see cref="RestClient" />.
    /// </summary>
    public class RestClientTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly RestClient _restClient;

        /// <summary>
        /// The client options, used for manipulating the situations in-test.
        /// </summary>
        private readonly RestClientOptions _options;

        /// <summary>
        /// The name of the client.
        /// </summary>
        private readonly string _fakeClientName;

        /// <summary>
        /// A mock of <see cref="IInternalClient"/>.
        /// </summary>
        private readonly Mock<IInternalClient> _mockHttpClient;

        /// <summary>
        /// A mock of <see cref="IMediaTypeFormatter"/>.
        /// </summary>
        private readonly Mock<IMediaTypeFormatter> _mockMediaTypeFormatter;

        /// <summary>
        /// A mock of <see cref="IInternalClientFactory"/>.
        /// </summary>
        private readonly Mock<IInternalClientFactory> _mockInternalClientFactory;

        /// <summary>
        /// A mock of <see cref="IHttpAlertingService"/>.
        /// </summary>
        private readonly Mock<IHttpAlertingService> _mockHttpAlerting;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RestClientTests()
        {
            _mockHttpAlerting = Moq.Create<IHttpAlertingService>();
            _mockInternalClientFactory = Moq.Create<IInternalClientFactory>();
            _mockHttpClient = Moq.Create<IInternalClient>();
            _mockMediaTypeFormatter = Moq.Create<IMediaTypeFormatter>();
            _mockMediaTypeFormatter.Setup(f => f.MediaTypes).Returns(new[] { MediaType.Json });
            var accessTokenCacheMock = Moq.Create<IAccessTokenCacheService>();
            var pollyContextFactoryMock = Moq.Create<IPollyContextFactory>();
            pollyContextFactoryMock.Setup(x => x.Create()).Returns(new Context());
            _fakeClientName = FakeData.Create<string>();
            var mockTelemetryFactory = Moq.Create<ITelemetryFactory>(MockBehavior.Loose);

            _options = new RestClientOptions.Builder("https://localhost:5000")
                .ConfigureHeader("Foo", "Bar")
                .ConfigureTimeout(TimeSpan.FromSeconds(7))
                .ConfigureJsonFormatting(StandardSerializerConfiguration.Settings)
                .ConfigureMiddleware(
                    r =>
                    {
                        r.Headers["Id"] = Guid.NewGuid().ToString();
                        return r;
                    })
                .ConfigureMiddlewareAsync(
                    async (restRequest, cancellationToken) =>
                    {
                        restRequest.UseHeader("x-jeremy-is", "awesomely-asynchronous");
                        await Task.Delay(1000, cancellationToken);
                        return restRequest;
                    })
                .ConfigureAlertCondition(CommonRestAlertConditions.None())
                .Build();

            _restClient = new RestClient(
                _fakeClientName,
                _mockInternalClientFactory.Object,
                _options,
                new[] { _mockMediaTypeFormatter.Object },
                pollyContextFactoryMock.Object,
                accessTokenCacheMock.Object,
                _mockHttpAlerting.Object,
                mockTelemetryFactory.Object);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync(Core.Rest.RestRequest,CancellationToken)" /> with all
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
                    r.Headers.Any(h => h.Key == "Id") &&
                    r.Headers.Any(h => h.Key == "x-jeremy-is"));
            SetupHttClientFactory();
            await SetupRestResponseAlertingNone(fakeHttpResponseMessage, fakeRequest);

            // act
            var actual = await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestResponse>(actual);
            Assert.Empty(actual.ResponseBytes);
            Assert.Equal(fakeHttpResponseMessage.IsSuccessStatusCode, actual.IsSuccessful);
            Assert.Equal(fakeHttpResponseMessage.StatusCode, actual.StatusCode);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync(RestRequest,CancellationToken)" />.<br/>
        /// Ensures when an alert condition is provided and met, an exception is thrown.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task SendAsync_WhenAlertMet_Throws()
        {
            // arrange
            var fakeRestRequest = new RestRequest("/hello/world", HttpMethod.Get)
                .UseAcceptHeader(MediaType.Json)
                .UseHeader("test", "testing")
                .ConfigureAlertCondition(new AlertCondition
                {
                    Condition = _ => true,
                    ThrowOnFailure = true
                });
            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            SetUpHttpClientMock(
                fakeHttpResponseMessage,
                r =>
                    r.Method == fakeRestRequest.HttpMethod &&
                    r.Headers.Accept.Any(h => h.MediaType == MediaType.Json.MimeType) &&
                    r.Headers.GetValues("test").First() == "testing" &&
                    r.RequestUri == new Uri("/hello/world", UriKind.Relative) &&
                    r.Headers.Any(h => h.Key == "Id") &&
                    r.Headers.Any(h => h.Key == "x-jeremy-is"));
            SetupHttClientFactory();
            var convertedResponse = await ConvertToRestResponse<RestResponse>(fakeHttpResponseMessage);
            _mockHttpAlerting
                .Setup(alert => alert.IsHttpAlert(
                    convertedResponse,
                    fakeRestRequest.AlertCondition,
                    CommonRestAlertConditions.None()))
                .Returns(true);
            _mockHttpAlerting
                .Setup(alert => alert.ThrowIfAlertedAndNeedingException(
                    fakeRestRequest.AlertCondition,
                    CommonRestAlertConditions.None()))
                .Throws(new AlertConditionMetException(fakeRestRequest.AlertCondition!.Exception));

            // act
            Func<Task> act = async () => await _restClient.SendAsync(fakeRestRequest, CancellationTokenSource.Token);

            // assert
            await act.Should().ThrowAsync<AlertConditionMetException>();
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RestClient.SendAsync(RestRequest,CancellationToken)" />.<br/>
        /// Ensures when an alert condition is provided but not met, no exception is thrown.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task SendAsync_WhenAlertNotMet_DoesntThrow()
        {
            // arrange
            var fakeRequest = new RestRequest("/hello/world", HttpMethod.Get)
                .UseAcceptHeader(MediaType.Json)
                .UseHeader("test", "testing");
            fakeRequest.ConfigureAlertCondition(CommonRestAlertConditions.None());
            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            SetUpHttpClientMock(
                fakeHttpResponseMessage,
                r =>
                    r.Method == fakeRequest.HttpMethod &&
                    r.Headers.Accept.Any(h => h.MediaType == MediaType.Json.MimeType) &&
                    r.Headers.GetValues("test").First() == "testing" &&
                    r.RequestUri == new Uri("/hello/world", UriKind.Relative) &&
                    r.Headers.Any(h => h.Key == "Id") &&
                    r.Headers.Any(h => h.Key == "x-jeremy-is"));
            SetupHttClientFactory();
            await SetupRestResponseAlertingNone(fakeHttpResponseMessage, fakeRequest);

            // act
            Func<Task> act = async () => await _restClient.SendAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            await act.Should().NotThrowAsync();
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
            await SetupRestResponseAlertingNone(fakeHttpResponseMessage, fakeRequest);

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
            await SetupFileResponseAlertingNone(mockResponse, fakeRequest);

            // act
            var actual = await _restClient.GetFileAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            actual.Should().BeOfType<FileResponse>();
            actual.FileStream.Length.Should().Be(expectedBytesStream.Length);
            actual.IsSuccessful.Should().Be(mockResponse.IsSuccessStatusCode);
            actual.StatusCode.Should().Be(mockResponse.StatusCode);
            actual.ContentType.Should().Be(mockResponse.Content.Headers.ContentType?.MediaType);
            actual.FileName.Should().Be(mockResponse.Content.Headers.ContentDisposition?.FileName);
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
            await SetupFileResponseAlertingNone(mockResponse, fakeRequest);

            // act
            var actual = await _restClient.GetFileAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            actual.Should().BeOfType<FileResponse>();
            actual.FileStream.Length.Should().Be(expectedBytesStream.Length);
            actual.IsSuccessful.Should().Be(mockResponse.IsSuccessStatusCode);
            actual.StatusCode.Should().Be(mockResponse.StatusCode);
            actual.ContentType.Should().Be(mockResponse.Content.Headers.ContentType?.MediaType);
            actual.FileName.Should().Be(mockResponse.Content.Headers.ContentDisposition?.FileName);
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
            await SetupRestResponseAlertingNone(fakeHttpResponseMessage, fakeRequest);

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
            await SetupFileResponseAlertingNone(mockResponse, fakeRequest);

            // act
            var actual = await _restClient.GetFileAsync(fakeRequest, CancellationTokenSource.Token);

            // assert
            actual.FileName.Should().Be(null);
        }

        /// <summary>
        /// Constructs a mock <see cref="FileResponse"/> based on the <see cref="MemoryStream"/> provided.
        /// </summary>
        /// <param name="expectedBytesStream">The provided <see cref="MemoryStream"/>.</param>
        /// <returns>
        /// The constructed <see cref="HttpResponseMessage"/> that will convert to a <see cref="FileResponse"/>.
        /// </returns>
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

        /// <summary>
        /// Sets up the underlying http client with the given expression matcher.
        /// </summary>
        /// <param name="fakeHttpResponseMessage">The <see cref="HttpResponseMessage"/> to respond with.</param>
        /// <param name="match">The expression to match with.</param>
        private void SetUpHttpClientMock(
            HttpResponseMessage fakeHttpResponseMessage,
            Expression<Func<HttpRequestMessage, bool>> match)
        {
            _mockHttpClient.Setup(
                    x => x.SendAsync(It.Is(match), CancellationTokenSource.Token))
                .ReturnsAsync(fakeHttpResponseMessage);
        }

        /// <summary>
        /// Sets up the underlying http client factory to produce a specified <see cref="HttpClient"/>.
        /// </summary>
        private void SetupHttClientFactory()
        {
            _mockInternalClientFactory.Setup(
                    x => x.CreateClient(_fakeClientName, _options.BaseUri, _options.Timeout))
                .Returns(_mockHttpClient.Object);
        }

        /// <summary>
        /// Sets up the <see cref="IHttpAlertingService"/> mock to handle a response that will convert to
        /// a <see cref="FileResponse"/>.
        /// </summary>
        /// <param name="fakeHttpResponseMessage">The mock response.</param>
        /// <param name="fakeRestRequest">The rest request.</param>
        private async Task SetupFileResponseAlertingNone(
            HttpResponseMessage fakeHttpResponseMessage,
            RestRequest fakeRestRequest)
        {
            var convertedResponse = await ConvertToStreamResponse(fakeHttpResponseMessage);
            _mockHttpAlerting
                .Setup(alert => alert.IsHttpAlert(
                    convertedResponse,
                    fakeRestRequest.AlertCondition,
                    CommonRestAlertConditions.None()))
                .Returns(false);
        }

        /// <summary>
        /// Sets up the <see cref="IHttpAlertingService"/> mock to handle a response that will convert to
        /// a <see cref="RestResponse"/>.
        /// </summary>
        /// <param name="fakeHttpResponseMessage">The mock response.</param>
        /// <param name="fakeRestRequest">The rest request.</param>
        private async Task SetupRestResponseAlertingNone(
            HttpResponseMessage fakeHttpResponseMessage,
            RestRequest fakeRestRequest)
        {
            var convertedResponse = await ConvertToRestResponse<RestResponse>(fakeHttpResponseMessage);
            _mockHttpAlerting
                .Setup(alert => alert.IsHttpAlert(
                    convertedResponse,
                    fakeRestRequest.AlertCondition,
                    CommonRestAlertConditions.None()))
                .Returns(false);
        }

        /// <summary>
        /// A copy of the method from the <see cref="RestClient"/> that converts to a <see cref="FileResponse"/>.
        /// </summary>
        /// <param name="httpResponseMessage">The response to convert.</param>
        /// <returns>The converted <see cref="FileResponse"/>.</returns>
        private async Task<FileResponse> ConvertToStreamResponse(HttpResponseMessage httpResponseMessage)
        {
            var responseStream = await httpResponseMessage.Content.ReadAsStreamAsync();

            return new FileResponse(
                httpResponseMessage.IsSuccessStatusCode,
                responseStream,
                httpResponseMessage.StatusCode,
                httpResponseMessage.Content.Headers.ContentType?.MediaType,
                httpResponseMessage.Content.Headers.ContentDisposition?.FileName);
        }

        /// <summary>
        /// A copy of the method from the <see cref="RestClient"/> that converts to a <see cref="RestResponse"/>.
        /// </summary>
        /// <param name="httpResponseMessage">The response to convert.</param>
        /// <param name="expectedResponse">The expected response shape in return.</param>
        /// <typeparam name="TResponse">The type of response.</typeparam>
        /// <returns>The converted <see cref="RestResponse"/>.</returns>
        private async Task<RestResponse> ConvertToRestResponse<TResponse>(
            HttpResponseMessage httpResponseMessage,
            TResponse expectedResponse = null)
                where TResponse : class
        {
            var responseBytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
            var deserializer = new TestRestResponseDeserializer(_ => expectedResponse, responseBytes);
            return new RestResponse(
                httpResponseMessage.IsSuccessStatusCode,
                deserializer,
                httpResponseMessage.StatusCode,
                httpResponseMessage.Headers.Location);
        }
    }
}