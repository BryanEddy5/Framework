using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.Soap.Resiliency;
using HumanaEdge.Webcore.Framework.Soap.Tests.Stubs;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Soap.Tests
{
    /// <summary>
    /// Unit tests for <see cref="SoapHttpMessageHandler"/>.
    /// </summary>
    public class SoapHttpMessageHandlerTests : BaseTests
    {
        /// <summary>
        /// An <see cref="HttpClient"/> with the SUT (<see cref="SoapHttpMessageHandler"/>) applied.
        /// </summary>
        private readonly HttpClient _soapHttpClient;

        /// <summary>
        /// Mock of <see cref="ITelemetryFactory"/>.
        /// </summary>
        private readonly Mock<ITelemetryFactory> _mockTelemetryFactory;

        /// <summary>
        /// Mock of <see cref="IPollyContextFactory"/>.
        /// </summary>
        private readonly Mock<IPollyContextFactory> _mockPollyContextFactory;

        /// <summary>
        /// Mock of <see cref="IHttpMessageHandler"/>.
        /// </summary>
        private readonly Mock<IHttpMessageHandler> _mockHttpHandler;

        /// <summary>
        /// Some SOAPey options to work with across our tests.
        /// </summary>
        private SoapClientOptions _soapClientOptions;

        /// <summary>
        /// Common setup code.
        /// </summary>
        public SoapHttpMessageHandlerTests()
        {
            _soapClientOptions = new SoapClientOptions.Builder("https://humana.com")
                .ConfigureTimeout(TimeSpan.FromSeconds(12))
                .ConfigureHeader("foo", "bar")
                .Build();

            _mockTelemetryFactory = Moq.Create<ITelemetryFactory>();
            _mockPollyContextFactory = Moq.Create<IPollyContextFactory>();
            _mockHttpHandler = Moq.Create<IHttpMessageHandler>();

            var fakeName = FakeData.Create<string>();
            var stubDelegatingHandler = new StubDelegatingHandler(_mockHttpHandler.Object);
            var soapHttpMessageHandler = new SoapHttpMessageHandler(
                fakeName,
                stubDelegatingHandler,
                _soapClientOptions,
                _mockTelemetryFactory.Object,
                _mockPollyContextFactory.Object);

            _soapHttpClient = new HttpClient(soapHttpMessageHandler);
        }

        /// <summary>
        /// Validates the behavior of <see cref="SoapHttpMessageHandler"/>- indirectly via an http client.<br/>
        /// Ensures that the basic functionality of sending works as expected.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Fact]
        public async Task SendAsync_IsSuccessful()
        {
            // arrange
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _soapClientOptions.BaseEndpoint);
            var fakeHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _mockHttpHandler.Setup(
                    c => c.SendAsync(
                        It.Is<HttpRequestMessage>(
                            r => r.Headers.GetValues("foo").First() == "bar" &&
                                 r.Method == HttpMethod.Get &&
                                 r.RequestUri == _soapClientOptions.BaseEndpoint),
                        It.IsAny<CancellationToken>())) // CancellationTokenSource.Token doesn't seem to work here?
                .ReturnsAsync(fakeHttpResponseMessage);

            SetupTelemetryTracking(
                httpRequestMessage.RequestUri!.ToString(),
                fakeHttpResponseMessage.StatusCode);

            // act
            var actual = await _soapHttpClient.SendAsync(
                httpRequestMessage,
                CancellationTokenSource.Token);

            // assert
            actual.Should().NotBeNull();
        }

        /// <summary>
        /// Sets up telemetry tracking for the provided data points.
        /// </summary>
        /// <param name="uri">The path of the request.</param>
        /// <param name="statusCode">The mock status code.</param>
        private void SetupTelemetryTracking(string uri, HttpStatusCode? statusCode)
        {
            _mockTelemetryFactory.Setup(
                t => t.Track(
                    It.Is<TelemetryEvent>(
                        telemetry =>
                            telemetry.Name == "HttpDependencyTelemetry" &&
                            telemetry.TelemetryType == TelemetryType.Dependency &&
                            telemetry.Tags["Uri"] as string == uri &&
                            (statusCode == null ||
                             telemetry.Tags["ResultCode"] as string == ((int?)statusCode).ToString()) &&
                            (bool)telemetry.Tags["Success"] == (statusCode != null && (int)statusCode < 400))));
        }
    }
}