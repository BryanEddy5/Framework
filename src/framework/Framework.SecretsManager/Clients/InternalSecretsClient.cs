using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.SecretManager.V1;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Core.SecretsManager.Converters;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Clients
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    internal sealed class InternalSecretsClient : IInternalSecretsClient
    {
        private static SecretManagerServiceClient? _client;

        /// <inheritdoc />
        public async Task<TSecret> GetAsync<TSecret>(
            SecretsKey secretsOptions,
            CancellationToken cancellationToken)
        {
            var result = await GetSecret(secretsOptions, cancellationToken);
            return JsonConvert.DeserializeObject<TSecret>(
                result.Payload.Data.ToStringUtf8(),
                StandardSerializerConfiguration.Settings)!;
        }

        /// <inheritdoc />
        public async Task<Stream> GetAsync(SecretsOptions secretsOptions, CancellationToken cancellationToken)
        {
            var result = await GetSecret(secretsOptions.ToSecretsKey(), cancellationToken);

            return new MemoryStream(result.Payload.Data.ToByteArray());
        }

        private async Task<AccessSecretVersionResponse> GetSecret(
            SecretsKey secretsOptions,
            CancellationToken cancellationToken)
        {
            _client ??= await SecretManagerServiceClient.CreateAsync(cancellationToken);

            return await _client.AccessSecretVersionAsync(
                new SecretVersionName(
                    secretsOptions.ProjectId,
                    secretsOptions.SecretId,
                    secretsOptions.SecretVersionId),
                cancellationToken);
        }
    }
}