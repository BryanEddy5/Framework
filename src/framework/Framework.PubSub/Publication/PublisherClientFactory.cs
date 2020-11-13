using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Grpc.Core;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <inheritdoc />
    internal sealed class PublisherClientFactory : IPublisherClientFactory
    {
        /// <summary>
        /// The designator for utilizing a pub/sub emulator.
        /// </summary>
        internal const string PubSubEmulatorEnvVarKey = "PUBSUB_EMULATOR_HOST";

        private static PublisherServiceApiClient? _client;

        /// <inheritdoc/>
        public async Task<IInternalPublisherClient> CreateAsync(TopicName topicName, CancellationToken cancellationToken)
        {
            if (_client != null)
            {
                return new InternalPublisherClient(_client);
            }

            var emulatorHostAndPort = Environment.GetEnvironmentVariable(PubSubEmulatorEnvVarKey);
            if (string.IsNullOrWhiteSpace(emulatorHostAndPort))
            {
                _client = await new PublisherServiceApiClientBuilder().BuildAsync(cancellationToken);
                return new InternalPublisherClient(_client);
            }

            _client = await new PublisherServiceApiClientBuilder
            {
                Endpoint = emulatorHostAndPort,
                ChannelCredentials = ChannelCredentials.Insecure
            }.BuildAsync(cancellationToken);

            try
            {
                await _client.GetTopicAsync(topicName, cancellationToken);
            }
            catch (RpcException)
            {
                // assuming that the exception is due to the topic not existing. Try to create it. If it fails here, exception will bubble up.
                await _client.CreateTopicAsync(topicName, cancellationToken);
            }

            return new InternalPublisherClient(_client);
        }
    }
}