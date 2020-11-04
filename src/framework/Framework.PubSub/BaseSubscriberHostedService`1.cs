using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.Telemetry;
using HumanaEdge.Webcore.Core.Telemetry.PubSub;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace HumanaEdge.Webcore.Framework.PubSub
{
    /// <summary>
    /// A hosted service for listening to a pubsub subscription.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to be received from the subscription.</typeparam>
    public abstract class BaseSubscriberHostedService<TMessage> : IHostedService
    {
        private readonly ILogger<BaseSubscriberHostedService<TMessage>> _logger;

        private readonly IOptionsMonitor<PubSubOptions> _config;

        private readonly ISubOrchestrationService<TMessage> _subOrchestrationService;

        private readonly ISubscriberClientFactory _subscriberClientFactory;

        private readonly SubscriptionName _subscriptionName;

        /// <summary>
        /// The manager for tracking telemetry.
        /// </summary>
        private readonly ITelemetryFactory _telemetryFactory;

        private SubscriberClient? _subscriber;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="telemetryFactory">The telemetry manager.</param>
        /// <param name="logger">A logger.</param>
        /// <param name="config">The configuration for processing the message.</param>
        /// <param name="subscriberClientFactory">A factory that generates a <see cref="SubscriberClient" />.</param>
        /// <param name="subOrchestrationService">
        /// A service that performs the business logic orchestration on the subscribed
        /// message.
        /// </param>
        public BaseSubscriberHostedService(
            ILogger<BaseSubscriberHostedService<TMessage>> logger,
            IOptionsMonitor<PubSubOptions> config,
            ISubscriberClientFactory subscriberClientFactory,
            ISubOrchestrationService<TMessage> subOrchestrationService,
            ITelemetryFactory telemetryFactory = null!)
        {
            _telemetryFactory = telemetryFactory;
            _logger = logger;
            _config = config;
            _subscriptionName = new SubscriptionName(config.CurrentValue.ProjectId, config.CurrentValue.Name);
            _subscriberClientFactory = subscriberClientFactory;
            _subOrchestrationService = subOrchestrationService;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscriber hosted service is running.");
            _subscriber = await _subscriberClientFactory.GetSubscriberClient(_subscriptionName, _config.CurrentValue);

            _ = _subscriber.StartAsync(
                async (message, cancel) =>
                {
                    var success = true;
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var messageData = message.Data.ToStringUtf8();
                    try
                    {
                        var deserializedMessage =
                            JsonConvert.DeserializeObject<TMessage>(messageData, StandardSerializerConfiguration.Settings);
                        await _subOrchestrationService.ExecuteAsync(deserializedMessage!, cancel);
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (JsonReaderException exception)
                    {
                        _logger.LogError(
                            exception,
                            "Message is not in the correct format to be parsed: {MessageId}.",
                            message.MessageId);
                        success = false;
                        return SubscriberClient.Reply.Ack;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(
                            exception,
                            "An exception occured for message: {MessageId}.",
                            message.MessageId);
                        success = false;
                        return SubscriberClient.Reply.Nack;
                    }
                    finally
                    {
                        stopwatch.Stop();
                        var duration = stopwatch.ElapsedMilliseconds;
                        TrackTelemetry(success, duration, message);
                    }
                });
        }

        /// <inheritdoc />
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriber?.StopAsync(cancellationToken) !;
        }

        /// <summary>
        /// Short-hand method for tracking telemetry in this Middleware.
        /// </summary>
        /// <param name="success">Designates if the request was successful.</param>
        /// <param name="duration">The total duration of the request.</param>
        /// <param name="message">The pubsub message.</param>
        private void TrackTelemetry(bool success, double duration, PubsubMessage message)
        {
            _telemetryFactory?.TrackSubscriptionTelemetry(
                DateTimeOffset.UtcNow,
                message.MessageId,
                duration,
                success);
        }
    }
}