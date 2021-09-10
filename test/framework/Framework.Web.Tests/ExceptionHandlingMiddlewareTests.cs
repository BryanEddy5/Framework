using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Core.Web.Contracts;
using HumanaEdge.Webcore.Core.Web.Exceptions;
using HumanaEdge.Webcore.Framework.Web.Exceptions;
using HumanaEdge.Webcore.Framework.Web.Options;
using HumanaEdge.Webcore.Framework.Web.Tests.Stubs.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.Web.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ExceptionHandlingMiddleware" /> class.
    /// </summary>
    public class ExceptionHandlingMiddlewareTests : BaseTests
    {
        /// <summary>
        /// Application logger.
        /// </summary>
        private Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;

        /// <summary>
        /// The configuration settings.
        /// </summary>
        private Mock<IOptionsMonitor<ExceptionHandlingOptions>> _mockOptions;

        /// <summary>
        /// A delegate used to invoke the next step in the web pipeline.
        /// </summary>
        private RequestDelegate _requestDelegate;

        /// <summary>
        /// SUT.
        /// </summary>
        private ExceptionHandlingMiddleware _systemUnderTest;

        /// <summary>
        /// Verifies the <see cref="HttpContext.Response" /> does not contain <see cref="DebugProblemDetail.Exception" />
        /// when <see cref="ExceptionHandlingOptions.ShowExceptionDetails" /> is set to false.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task InvokeAsync_HasExceptionInBody_When_ShowExceptionDetailIsFalse()
        {
            // arrange
            var httpContext = CreateHttpContext();
            SetupForOptions(false, new FakeNotFoundMessageException());

            // act
            await _systemUnderTest.InvokeAsync(httpContext);
            var actual = await ResponseBodyToString(httpContext);

            // assert
            Assert.True(httpContext.Response.Body.Length > 0);
            Assert.Null(actual["exception"]);
            Assert.NotNull(actual["message"]);
            Assert.Equal(FakeNotFoundMessageException.ExceptionMessage, actual["message"]);
        }

        /// <summary>
        /// Verifies the <see cref="HttpContext.Response" /> does not contain <see cref="MessageAppException.LoggedMessage" />.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task InvokeAsync_ResponseDoesNotContainLoggedMessage()
        {
            // arrange
            var httpContext = CreateHttpContext();
            SetupForOptions(false, new FakeComplexLoggingException());

            // act
            await _systemUnderTest.InvokeAsync(httpContext);
            var actual = await ResponseBodyToString(httpContext);

            // assert
            Assert.True(httpContext.Response.Body.Length > 0);
            Assert.Null(actual["exception"]);
            Assert.NotNull(actual["message"]);
            actual["message"].ToString().Should().NotContain(FakeComplexLoggingException.ExceptionLoggedMessage);
            actual["message"].ToString().Should().Contain(FakeComplexLoggingException.ExceptionMessage);
            Assert.Equal(FakeNotFoundMessageException.ExceptionMessage, actual["message"]);
        }

        /// <summary>
        /// Verifies the <see cref="HttpContext.Response" /> does contain <see cref="DebugProblemDetail.Exception" />
        /// when <see cref="ExceptionHandlingOptions.ShowExceptionDetails" /> is set to true.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task InvokeAsync_HasExceptionInBody_When_ShowExceptionDetailIsTrue()
        {
            // arrange
            var httpContext = CreateHttpContext();
            SetupForOptions(true, new FakeNotFoundMessageException());

            // act
            await _systemUnderTest.InvokeAsync(httpContext);
            var actual = await ResponseBodyToString(httpContext);

            // assert
            Assert.True(httpContext.Response.Body.Length > 0);
            Assert.NotNull(actual["exception"]);
            Assert.Contains("StackTrace", actual["exception"].ToString());
        }

        /// <summary>
        /// Verifies no exception is thrown for <see cref="ExceptionHandlingMiddleware.InvokeAsync" /> and the
        /// response is empty.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task InvokeAsync_HttpContextResponse_ShouldBeEmpty()
        {
            // arrange
            _mockLogger = Moq.Create<ILogger<ExceptionHandlingMiddleware>>(MockBehavior.Loose);
            _mockOptions = Moq.Create<IOptionsMonitor<ExceptionHandlingOptions>>();
            _requestDelegate = innerHttpContext => Task.CompletedTask;
            _systemUnderTest = new ExceptionHandlingMiddleware(
                _requestDelegate,
                _mockLogger.Object,
                _mockOptions.Object);
            var httpContext = new DefaultHttpContext();

            // act
            await _systemUnderTest.InvokeAsync(httpContext);

            // assert
            Assert.Equal(0, httpContext.Response.Body.Length);
        }

        /// <summary>
        /// Verifies an http status code <see cref="HttpStatusCode.InternalServerError" /> is when
        /// <see cref="Exception" /> is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task InvokeAsync_Throws_NonMessageAppException_ShouldReturn_500()
        {
            // arrange
            var httpContext = CreateHttpContext();
            SetupForOptions(false, new Exception());
            var expected = (int)HttpStatusCode.InternalServerError;

            // act
            await _systemUnderTest.InvokeAsync(httpContext);

            // assert
            Assert.Equal(expected, httpContext.Response.StatusCode);
        }

        /// <summary>
        /// Verifies an http status code <see cref="HttpStatusCode.NotFound" /> is when
        /// <see cref="FakeNotFoundMessageException" /> is thrown.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task InvokeAsync_Throws_NotFoundException()
        {
            // arrange
            var httpContext = CreateHttpContext();
            SetupForOptions(false, new FakeNotFoundMessageException());
            var expected = (int)HttpStatusCode.NotFound;

            // act
            await _systemUnderTest.InvokeAsync(httpContext);

            // assert
            Assert.Equal(expected, httpContext.Response.StatusCode);
        }

        /// <summary>
        /// Verifies an http status code <see cref="HttpStatusCode.NotFound"/> is returned when
        /// <see cref="AggregateException"/> containing a <see cref="FakeNotFoundMessageException"/> is thrown.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task InvokeAsync_ThrowsAggregateException()
        {
            // arrange
            var httpContext = CreateHttpContext();
            var exception = new AggregateException(new[] { new FakeNotFoundMessageException() });
            SetupForOptions(false, exception);
            var expected = (int)HttpStatusCode.NotFound;

            // act
            await _systemUnderTest.InvokeAsync(httpContext);

            // assert
            Assert.Equal(expected, httpContext.Response.StatusCode);
        }

        /// <summary>
        /// Verifies an http status code <see cref="HttpStatusCode.BadRequest"/> is returned when
        /// <see cref="BadRequestException"/> containing a <see cref="BadRequestResponse"/> is thrown.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task InvokeAsync_ThrowsBadRequestException()
        {
            // arrange
            var httpContext = CreateHttpContext();
            var fakeBadRequest = FakeData.Create<BadRequestResponse>();
            SetupForOptions(false, new BadRequestException(fakeBadRequest), false);
            Moq.VerifyNoOtherCalls();

            // act
            await _systemUnderTest.InvokeAsync(httpContext);
            var actual = await ResponseBodyToString(httpContext);

            // assert
            Assert.True(httpContext.Response.Body.Length > 0);
            actual["errors"].ToString().Should().NotBeNullOrEmpty();
            actual["title"].ToString().Should().Be(fakeBadRequest.Title);
            actual["type"].ToString().Should().Be(fakeBadRequest.Type);
            actual["status"].ToString().Should().Be(fakeBadRequest.Status.ToString());
        }

        /// <summary>
        /// Builds the context for testing.
        /// </summary>
        /// <returns>HttpContext.</returns>
        private static HttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            return context;
        }

        /// <summary>
        /// Converts the <see cref="HttpContext" /> response body into a <see cref="JToken" />.
        /// </summary>
        /// <param name="httpContext">Contains information about the http request.</param>
        /// <returns>Returns a <see cref="JToken" />.</returns>
        private static async Task<JToken> ResponseBodyToString(HttpContext httpContext)
        {
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyStreamReader = new StreamReader(httpContext.Response.Body);
            var response = await responseBodyStreamReader.ReadToEndAsync();
            return JObject.Parse(response);
        }

        /// <summary>
        /// Creates a common setup for <see cref="IOptionsMonitor{ExceptionHandlingOptions}" />.
        /// </summary>
        /// <param name="showExceptionDetails">A flag to determine if the exception stack trace should be shown.</param>
        /// <param name="exception">A configurable exception to be thrown.</param>
        /// <param name="setupOptions">Setup options mock. </param>
        private void SetupForOptions(bool showExceptionDetails, Exception exception, bool setupOptions = true)
        {
            _mockLogger = Moq.Create<ILogger<ExceptionHandlingMiddleware>>(MockBehavior.Loose);
            _mockOptions = Moq.Create<IOptionsMonitor<ExceptionHandlingOptions>>();
            _requestDelegate = innerHttpContext => throw exception;
            _systemUnderTest = new ExceptionHandlingMiddleware(
                _requestDelegate,
                _mockLogger.Object,
                _mockOptions.Object);
            var fakeOptions = FakeData.Build<ExceptionHandlingOptions>()
                .With(x => x.ShowExceptionDetails, showExceptionDetails)
                .Create();
            if (setupOptions)
            {
                _mockOptions.Setup(x => x.CurrentValue).Returns(fakeOptions);
            }
        }
    }
}