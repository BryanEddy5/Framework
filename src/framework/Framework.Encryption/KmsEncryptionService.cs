using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Kms.V1;
using Google.Protobuf;
using HumanaEdge.Webcore.Core.Encryption;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.Encryption
{
    /// <summary>
    /// Service that uses GCP's KMS to encrypt/decrypt data.
    /// </summary>
    internal sealed class KmsEncryptionService : IEncryptionService
    {
        private readonly IOptionsMonitor<EncryptionServiceOptions> _options;

        private readonly IKeyManagementServiceClientFactory _kmsClientFactory;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="options">Options object with config values for reaching KMS instance.</param>
        /// <param name="kmsClientFactory">Factory for generating KMS clients.</param>
        public KmsEncryptionService(
            IOptionsMonitor<EncryptionServiceOptions> options,
            IKeyManagementServiceClientFactory kmsClientFactory)
        {
            _options = options;
            _kmsClientFactory = kmsClientFactory;
        }

        /// <summary>
        /// Pulls out relevant options info to create a CryptoKeyName object.
        /// </summary>
        /// <param name="options">The EncryptionServiceOptions containing the required information to name the key.</param>
        /// <returns>The fully qualified CryptoKeyName object based on the passed in options.</returns>
        public static CryptoKeyName GetCryptoKeyName(EncryptionServiceOptions options)
        {
            return new CryptoKeyName(
                options.ProjectId,
                options.LocationId,
                options.KeyRingId,
                options.KeyId);
        }

        /// <inheritdoc />
        public async Task<string> EncryptSymmetric(string message)
        {
            var plaintext = Encoding.UTF8.GetBytes(message);

            var client = await _kmsClientFactory.CreateAsync();
            var result = await client.EncryptAsync(GetCryptoKeyName(_options.CurrentValue), ByteString.CopyFrom(plaintext));
            return WebEncoders.Base64UrlEncode(result.Ciphertext.ToByteArray());
        }

        /// <inheritdoc />
        public async Task<string> DecryptSymmetric(string cipherText)
        {
            var cipherTextBytes = WebEncoders.Base64UrlDecode(cipherText);

            var client = await _kmsClientFactory.CreateAsync();
            var result = await client.DecryptAsync(GetCryptoKeyName(_options.CurrentValue), ByteString.CopyFrom(cipherTextBytes));
            var plaintext = result.Plaintext.ToByteArray();
            return Encoding.UTF8.GetString(plaintext);
        }
    }
}