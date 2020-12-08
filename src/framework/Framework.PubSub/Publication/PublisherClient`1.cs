using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.PubSub;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <inheritdoc />
    internal sealed class PublisherClient<TMessage> : IPublisherClient<TMessage>
    {
        private readonly ILogger<PublisherClient<TMessage>> _logger;

        private readonly IPublisherClientFactory _publisherClientFactory;

        private readonly IPublishRequestConverter _publishRequestConverter;

        private readonly ITelemetryFactory _telemetryFactory;

        private readonly TopicName _topicName;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherClient" /> class.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        /// <param name="logger">The app logger.</param>
        /// <param name="publisherClientFactory">Factory pattern for creating the client.</param>
        /// <param name="publishRequestConverter">A service for generating the request.</param>
        /// <param name="telemetryFactory">Produces telemetry data for alerting and monitoring.</param>
        public PublisherClient(
            IOptionsMonitor<PublisherOptions> options,
            ILogger<PublisherClient<TMessage>> logger,
            IPublisherClientFactory publisherClientFactory,
            IPublishRequestConverter publishRequestConverter,
            ITelemetryFactory telemetryFactory)
        {
            var config = options.Get(typeof(TMessage).Name);
            _topicName = new TopicName(config.ProjectId, config.TopicName);
            _logger = logger;
            _publisherClientFactory = publisherClientFactory;
            _publishRequestConverter = publishRequestConverter;
            _telemetryFactory = telemetryFactory;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<string>> PublishAsync(
            TMessage[] message,
            CancellationToken cancellationToken)
        {
            var publisher = await _publisherClientFactory.CreateAsync(_topicName, cancellationToken);
            var publishRequests = message.Select(msg => _publishRequestConverter.Create(msg, _topicName));
            var messageIds = new ConcurrentBag<string>();

            var publishResponse = publishRequests.Select(
                async request =>
                {
                    var stopWatch = new Stopwatch();
                    string messageId = null !;
                    var success = false;
                    try
                    {
                        stopWatch.Start();
                        var response = await publisher.PublishAsync(
                            request,
                            cancellationToken);
                        messageId = response.MessageIds.FirstOrDefault();
                        if (messageId != null)
                        {
                            messageIds.Add(messageId);
                        }

                        success = true;

                        _logger.LogInformation("Published to {Topic} {@PublishResponse}", _topicName, response);
                    }
                    catch (RpcException rpcException)
                    {
                        throw new PublishException("Publishing message failed", rpcException);
                    }
                    finally
                    {
                        stopWatch.Stop();
                        var duration = stopWatch.ElapsedMilliseconds;
                        _telemetryFactory.TrackPublicationTelemetry(
                            DateTimeOffset.UtcNow,
                            messageId !,
                            duration,
                            success);
                    }
                });
            await Task.WhenAll(publishResponse);
            return messageIds.ToArray();
        }

        /// <inheritdoc/>
        public async Task<string> PublishAsync(TMessage message, CancellationToken cancellationToken) =>
            (await PublishAsync(new[] { message }, cancellationToken)).First();
    }
}