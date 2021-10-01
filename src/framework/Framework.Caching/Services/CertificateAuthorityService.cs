using System.Diagnostics.CodeAnalysis;
using Google.Cloud.SecretManager.V1;
using HumanaEdge.Webcore.Core.Caching.Options;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.Caching.Services
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    internal sealed class CertificateAuthorityService
        : ICertificateAuthorityService
    {
        /// <summary>
        /// Always use the latest version of the secret.
        /// </summary>
        private const string SecretVersionId = "latest";

        private static SecretManagerServiceClient? _client;

        private readonly IOptionsMonitor<CacheOptions> _options;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="options">The cache configuration options.</param>
        public CertificateAuthorityService(IOptionsMonitor<CacheOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// Retrieves the certificate authority.
        /// </summary>
        /// <returns>The base 64 encoded cert.</returns>
        public byte[] GetCertificate()
        {
            _client ??= SecretManagerServiceClient.Create();

            return _client.AccessSecretVersion(
                    new SecretVersionName(
                        _options.CurrentValue.CertificateAuthority.ProjectId,
                        _options.CurrentValue.CertificateAuthority.SecretId,
                        SecretVersionId))
                .Payload.Data.ToByteArray();
        }
    }
}