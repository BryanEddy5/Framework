using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions.Common;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Storage;
using HumanaEdge.Webcore.Core.Testing;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling.Converter;
using HumanaEdge.Webcore.Framework.PubSub.Tests.Stubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace HumanaEdge.Webcore.Framework.PubSub.Tests.Subscription.ExceptionHandling
{
    /// <summary>
    /// Unit tests for <see cref="ExceptionStorageService"/>.
    /// </summary>
    public class ExceptionStorageServiceTests : BaseTests
    {
        private readonly Mock<IOptionsMonitor<PubSubOptions>> _optionsMonitorMock;

        private readonly Mock<ILogger<ExceptionStorageService>> _loggerMock;

        private readonly Mock<IStorageClient> _storageClientMock;

        private readonly PubSubOptions _options;

        private readonly ExceptionStorageService _exceptionStorageService;

        /// <summary>
        /// Common test setup.
        /// </summary>
        public ExceptionStorageServiceTests()
        {
            _optionsMonitorMock = Moq.Create<IOptionsMonitor<PubSubOptions>>();
            _options = FakeData.Create<PubSubOptions>();
            _optionsMonitorMock.Setup(x => x.Get(typeof(Foo).FullName)).Returns(_options);
            _loggerMock = Moq.Create<ILogger<ExceptionStorageService>>(MockBehavior.Loose);
            _storageClientMock = Moq.Create<IStorageClient>();
            _exceptionStorageService = new ExceptionStorageService(
                _optionsMonitorMock.Object,
                _loggerMock.Object,
                _storageClientMock.Object);
            Activity.Current = new Activity("testing");
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionStorageService.LoadException{TMessage}"/>.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task LoadException()
        {
            // arrange
            var fakePayload = "{\n\"name\":\"This is bryan\"\n}";
            var fakeException = new Exception();
            var expectedMessage = fakeException.ToExceptionStorageMessage(
                fakePayload,
                _options.ExceptionStorage.ApplicationName!) as MemoryStream;
            _storageClientMock.Setup(
                    x => x.UploadObjectAsync(
                        It.Is<string>(x => x.EndsWith(_options.ExceptionStorage.ApplicationName)),
                        ExceptionStorageService.ContentType,
                        It.IsAny<MemoryStream>(),
                        _options.ExceptionStorage,
                        CancellationTokenSource.Token))
                .Returns(Task.CompletedTask);

            // act assert
            await _exceptionStorageService.LoadException<Foo>(
                fakePayload,
                fakeException,
                CancellationTokenSource.Token);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionStorageService.LoadException{TMessage}"/> when
        /// the appropriate parameters are not supplied. The storage service should not be invoked.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// /// <param name="gcpBucket">The name of the bucket.</param>
        /// /// <param name="gcpProject">The name of the project.</param>
        /// <returns>An awaitable task.</returns>
        [Theory]
        [InlineData("test", "test", null)]
        [InlineData(null, "test", null)]
        [InlineData("test", null, "test")]
        public async Task LoadException_MissingArguments(string applicationName, string gcpBucket, string gcpProject)
        {
            // arrange
            var fakePayload = "{\n\"name\":\"This is bryan\"\n}";
            var fakeException = new Exception();
            _options.ExceptionStorage.ApplicationName = applicationName;
            _options.ExceptionStorage.GcpBucket = gcpBucket;
            _options.ExceptionStorage.GcpProject = gcpProject;

            // act assert
            await _exceptionStorageService.LoadException<Foo>(
                fakePayload,
                fakeException,
                CancellationTokenSource.Token);
        }

        /// <summary>
        /// Verifies the behavior of <see cref="ExceptionStorageService.LoadException{TMessage}"/> when an exception is thrown.
        /// The service should log the exception and not throw exceptions under any circumstances.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        [Fact]
        public async Task LoadException_ExceptionThrown()
        {
            // arrange
            var fakePayload = "{\n\"name\":\"This is bryan\"\n}";
            var fakeException = new Exception();
            var expectedMessage = fakeException.ToExceptionStorageMessage(
                fakePayload,
                _options.ExceptionStorage.ApplicationName!) as MemoryStream;
            _storageClientMock.Setup(
                    x => x.UploadObjectAsync(
                        It.Is<string>(x => x.EndsWith(_options.ExceptionStorage.ApplicationName)),
                        ExceptionStorageService.ContentType,
                        It.IsAny<MemoryStream>(),
                        _options.ExceptionStorage,
                        CancellationTokenSource.Token))
                .ThrowsAsync(new Exception());

            // act assert
            await _exceptionStorageService.LoadException<Foo>(
                fakePayload,
                fakeException,
                CancellationTokenSource.Token);
        }
    }
}