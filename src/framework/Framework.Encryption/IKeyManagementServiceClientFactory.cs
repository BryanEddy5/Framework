using System.Threading.Tasks;
using Google.Cloud.Kms.V1;

namespace HumanaEdge.Webcore.Framework.Encryption
{
    /// <summary>
    /// Thin wrapper around Google's KMS API to allow for unit testing.
    /// </summary>
    public interface IKeyManagementServiceClientFactory
    {
        /// <summary>
        /// Create a KeyManagementServiceClient.
        /// </summary>
        /// <returns>A KeyManagementServiceClient.</returns>
        Task<KeyManagementServiceClient> CreateAsync();
    }
}