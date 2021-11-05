using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Common.Alerting;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Rest.Transformations;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using RestRequest = HumanaEdge.Webcore.Core.Rest.RestRequest;

namespace HumanaEdge.Webcore.Framework.Rest.Tests.Transformations
{
    /// <summary>
    /// Unit tests for <see cref="RequestTransformationService" />.
    /// </summary>
    public class RequestTransformationServiceTests : BaseTests
    {
        /// <summary>
        /// SUT.
        /// </summary>
        private readonly RequestTransformationService _requestTransformationService;

        /// <summary>
        /// A mock of <see cref="IAccessTokenCacheService"/>.
        /// </summary>
        private readonly Mock<IAccessTokenCacheService> _mockAccessTokenCache;

        /// <summary>
        /// A mock of <see cref="IHttpContextAccessor"/>.
        /// </summary>
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        private readonly string _authHeader;
        private readonly string _traceHeader;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public RequestTransformationServiceTests()
        {
            _mockAccessTokenCache = Moq.Create<IAccessTokenCacheService>(MockBehavior.Loose);
            _mockHttpContextAccessor = Moq.Create<IHttpContextAccessor>(MockBehavior.Loose);
            var mockMediaTypeFormatter = Moq.Create<IMediaTypeFormatter>(MockBehavior.Loose);

            _authHeader = FakeData.Create<string>();
            _traceHeader = FakeData.Create<string>();

            var options = new RestClientOptions.Builder("https://localhost:5000")
                .ConfigureHeader("Foo", "Bar")
                .ConfigureTimeout(TimeSpan.FromSeconds(7))
                .ConfigureBearerToken<string>(async r => await GetTokenAsync())
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

            _requestTransformationService = new RequestTransformationService(
                _mockAccessTokenCache.Object,
                _mockHttpContextAccessor.Object,
                new[] { mockMediaTypeFormatter.Object },
                options);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RequestTransformationService.TransformRequest{TRestRequest}(TRestRequest, CancellationToken)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TransformRequest_ReturnsTransformedRequest()
        {
            // arrange
            _mockAccessTokenCache
                .Setup(x => x.GetAsync(It.IsAny<Func<CancellationToken, Task<string>>>(), It.IsAny<string>(), CancellationTokenSource.Token, false))
                .ReturnsAsync(_authHeader);

            var context = new DefaultHttpContext();
            context.Request.Headers["x-cloud-trace-context"] = _traceHeader;
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);

            var fakeRestRequest = new RestRequest("/hello/world", HttpMethod.Get)
                .UseAcceptHeader(MediaType.Json)
                .UseHeader("test", "testing")
                .ConfigureAlertCondition(
                    new AlertCondition<BaseRestResponse>
                    {
                        Condition = _ => true,
                        ThrowOnFailure = true
                    });

            // act
            var actual = await _requestTransformationService.TransformRequest(fakeRestRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestRequest>(actual);
            actual.Headers.Should().ContainKey("x-cloud-trace-context");
            actual.Headers.Should().ContainKey("Foo");
            actual.Headers.Should().ContainKey("Authorization");
            actual.Headers.Should().ContainKey("test");
            actual.Headers.Should().ContainKey("x-jeremy-is");
            actual.Headers.Should().ContainKey("Foo");
            actual.Headers.Should().ContainKey("Accept");
            actual.Headers.Should().HaveCount(7);

            actual.Headers["x-cloud-trace-context"].Should().Equal(_traceHeader);
            actual.Headers["Authorization"].Should().Equal("Bearer " + _authHeader);
        }

                /// <summary>
        /// Verifies the behavior of <see cref="RequestTransformationService.TransformRequest{TRestRequest}(TRestRequest, CancellationToken)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task TransformRequest_NullHttpContext_ReturnsTransformedRequest()
        {
            // arrange
            _mockAccessTokenCache
                .Setup(x => x.GetAsync(It.IsAny<Func<CancellationToken, Task<string>>>(), It.IsAny<string>(), CancellationTokenSource.Token, false))
                .ReturnsAsync(_authHeader);

            var fakeRestRequest = new RestRequest("/hello/world", HttpMethod.Get)
                .UseAcceptHeader(MediaType.Json)
                .UseHeader("test", "testing")
                .ConfigureAlertCondition(
                    new AlertCondition<BaseRestResponse>
                    {
                        Condition = _ => true,
                        ThrowOnFailure = true
                    });

            // act
            var actual = await _requestTransformationService.TransformRequest(fakeRestRequest, CancellationTokenSource.Token);

            // assert
            Assert.IsType<RestRequest>(actual);
            actual.Headers.Should().ContainKey("Foo");
            actual.Headers.Should().ContainKey("Authorization");
            actual.Headers.Should().ContainKey("test");
            actual.Headers.Should().ContainKey("x-jeremy-is");
            actual.Headers.Should().ContainKey("Foo");
            actual.Headers.Should().ContainKey("Accept");
            actual.Headers.Should().HaveCount(6);
            actual.Headers["Authorization"].Should().Equal("Bearer " + _authHeader);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RequestTransformationService.ConvertToHttpRequestMessage(RestRequest)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
        /// </summary>
        [Fact]
        public void ConvertToRestResponse_ConvertsHttpResponseMsg()
        {
            // arrange
            var fakeRestRequest = new RestRequest("/hello/world", HttpMethod.Get)
                .UseAcceptHeader(MediaType.Json)
                .UseHeader("test", "testing")
                .ConfigureAlertCondition(
                    new AlertCondition<BaseRestResponse>
                    {
                        Condition = _ => true,
                        ThrowOnFailure = true
                    });

            // act
            var actual = _requestTransformationService.ConvertToHttpRequestMessage(fakeRestRequest);

            // assert
            Assert.IsType<HttpRequestMessage>(actual);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RequestTransformationService.ConvertToRestResponse(HttpResponseMessage)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ConvertToRestResponse_ConvertsHttpResponseMsg_ReturnsRestResponse()
        {
            // arrange
            var fakeRestRequest = new HttpResponseMessage();

            // act
            var actual = await _requestTransformationService.ConvertToRestResponse(fakeRestRequest);

            // assert
            Assert.IsType<RestResponse>(actual);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="RequestTransformationService.ConvertToStreamResponse(HttpResponseMessage)" /> with all
        /// <see cref="RestClientOptions" /> containing values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task ConvertToStreamResponse_ConvertsHttpResponseMsg_ReturnsFileResponse()
        {
            // arrange
            var mockContent = Moq.Create<HttpContent>(MockBehavior.Loose);
            var fakeContent = mockContent.Object;
            var fakeRestRequest = new HttpResponseMessage { Content = fakeContent };

            // act
            var actual = await _requestTransformationService.ConvertToStreamResponse(fakeRestRequest);

            // assert
            Assert.IsType<FileResponse>(actual);
        }

        private async Task<string> GetTokenAsync()
        {
            await Task.Delay(0);
            return _authHeader;
        }
    }
}