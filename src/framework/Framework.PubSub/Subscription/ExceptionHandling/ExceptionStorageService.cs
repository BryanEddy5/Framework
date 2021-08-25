using System;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Storage;
using HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling.Converter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.ExceptionHandling
{
    /// <inheritdoc />
    internal sealed class ExceptionStorageService
        : IExceptionStorageService
    {
        /// <summary>
        /// The content type of the file being uploaded.
        /// </summary>
        internal const string ContentType = "application/json";

        private readonly ILogger<ExceptionStorageService> _logger;

        private readonly IOptionsMonitor<PubSubOptions> _options;

        private readonly IStorageClient _storageClient;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="options"> Config settings. </param>
        /// <param name="logger"> App logger. </param>
        /// <param name="storageClient"> Client for uploading objects. </param>
        public ExceptionStorageService(
            IOptionsMonitor<PubSubOptions> options,
            ILogger<ExceptionStorageService> logger,
            IStorageClient storageClient)
        {
            _options = options;
            _logger = logger;
            _storageClient = storageClient;
        }

        /// <inheritdoc />
        public async Task LoadException<TMessage>(
            string payload,
            Exception exception,
            CancellationToken cancellationToken)
        {
            try
            {
                var exceptionStorage = _options.Get(typeof(TMessage).FullName).ExceptionStorage;
                var bucket = exceptionStorage.GcpBucket;
                var project = exceptionStorage.GcpProject;
                var applicationName = exceptionStorage.ApplicationName;
                if (bucket is null || project is null || applicationName is null)
                {
                    _logger.LogError(
                        "The exception cannot be published because either the {BucketName}, {ProjectId}, or {ApplicationName} is null",
                        bucket,
                        project,
                        applicationName);
                    return;
                }

                var fileName =
                    $"{DateTimeOffset.UtcNow:o}-{applicationName}";
                await using var stream = exception.ToExceptionStorageMessage(payload, applicationName!);

                await _storageClient.UploadObjectAsync(
                    fileName,
                    ContentType,
                    stream,
                    exceptionStorage,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an issue uploading to the exception storage bucket.");
            }
        }
    }
}