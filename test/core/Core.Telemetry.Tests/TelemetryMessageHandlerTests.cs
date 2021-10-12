using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Telemetry.Http;
using HumanaEdge.Webcore.Core.Testing;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Core.Telemetry.Tests
{
    /// <summary>
    /// Unit tests for <see cref="TelemetryMessageHandler"/>.
    /// </summary>
    public class TelemetryMessageHandlerTests : BaseTests
    {
        private readonly Mock<ITelemetryFactory> _telemetryFactoryMock;

        private TelemetryMessageHandler _telemetryMessageHandler;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public TelemetryMessageHandlerTests()
        {
            _telemetryFactoryMock = Moq.Create<ITelemetryFactory>();
            _telemetryMessageHandler = new TelemetryMessageHandler(_telemetryFactoryMock.Object);
        }

        /// <summary>
        /// Verifies the behavior <see cref="TelemetryMessageHandler.SendAsync"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SendAsync_Success()
        {
            // arrange
            var request = new HttpRequestMessage()
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Get
            };
            var expected = Setup(false, HttpStatusCode.InternalServerError);

            _telemetryFactoryMock.Setup(
                x => x.Track(
                    It.Is<TelemetryEvent>(
                        y => y.Alert == expected.Alert &&
                             (bool)y.Tags["Success"] == (bool)expected.Tags["Success"] &&
                             (string)y.Tags["ResultCode"] == (string)expected.Tags["ResultCode"])));

            var invoker = new HttpMessageInvoker(_telemetryMessageHandler);

            // act assert
            await invoker.SendAsync(request, CancellationTokenSource.Token);
        }

        /// <summary>
        /// Verifies the behavior <see cref="TelemetryMessageHandler.SendAsync"/> when the status code is not successful.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task SendAsync_Fail()
        {
            // arrange
            var request = new HttpRequestMessage()
            {
                Content = new StringContent(string.Empty, Encoding.UTF8, "application/json"),
                Method = HttpMethod.Get
            };
            var expected = Setup(false, HttpStatusCode.InternalServerError);

            _telemetryFactoryMock.Setup(
                x => x.Track(
                    It.Is<TelemetryEvent>(
                        y => y.Alert == expected.Alert &&
                             (bool)y.Tags["Success"] == (bool)expected.Tags["Success"] &&
                             (string)y.Tags["ResultCode"] == (string)expected.Tags["ResultCode"])));

            var invoker = new HttpMessageInvoker(_telemetryMessageHandler);

            // act assert
            await invoker.SendAsync(request, CancellationTokenSource.Token);
        }

        private TelemetryEvent Setup(
            bool isSuccess,
            HttpStatusCode httpStatusCode)
        {
            _telemetryMessageHandler.InnerHandler = new TestHandler(
                new HttpResponseMessage()
                {
                    StatusCode = httpStatusCode,
                    Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
                });
            return new DependencyHttpTelemetry(
                DateTimeOffset.UtcNow,
                0,
                ((int?)httpStatusCode)?.ToString()!,
                HttpMethod.Get.ToString(),
                null,
                isSuccess,
                null,
                !isSuccess).ToTelemetryEvent();
        }

        private class TestHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _responseMessage;

            public TestHandler(HttpResponseMessage responseMessage)
            {
                _responseMessage = responseMessage;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(_responseMessage);
            }
        }
    }
}