using System.Threading.Tasks;
using Google.Cloud.Kms.V1;

namespace HumanaEdge.Webcore.Framework.Encryption
{
    /// <inheritdoc />
    internal sealed class KeyManagementServiceClientFactory : IKeyManagementServiceClientFactory
    {
        private KeyManagementServiceClient? _keyManagementServiceClient;

        /// <inheritdoc />
        public async Task<KeyManagementServiceClient> CreateAsync()
        {
            return _keyManagementServiceClient ??= await KeyManagementServiceClient.CreateAsync();
        }
    }
}