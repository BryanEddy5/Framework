using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.SecretManager.V1;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

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
            _client ??= await SecretManagerServiceClient.CreateAsync(cancellationToken);

            var result = await _client.AccessSecretVersionAsync(
                new SecretVersionName(
                    secretsOptions.ProjectId,
                    secretsOptions.SecretId,
                    secretsOptions.SecretVersionId),
                cancellationToken);
            return JsonConvert.DeserializeObject<TSecret>(result.Payload.Data.ToStringUtf8(), StandardSerializerConfiguration.Settings) !;
        }
    }
}