using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Framework.Caching.Services
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    internal sealed class CertificateValidationFactory
        : ICertificateValidationFactory
    {
        private readonly ILogger<CertificateValidationFactory> _logger;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="logger">App logger. </param>
        public CertificateValidationFactory(ILogger<CertificateValidationFactory> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public RemoteCertificateValidationCallback Create()
        {
            return (
                sender,
                certificate,
                chain,
                sslPolicyErrors) =>
            {
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) ==
                    SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    var redisCertificateAuthority = (X509Certificate2)certificate!;

                    var caFound = chain != null &&
                                  chain.ChainElements
                                      .Cast<X509ChainElement>()
                                      .Any(x => x.Certificate.Thumbprint == redisCertificateAuthority.Thumbprint);
                    if (!caFound)
                    {
                        _logger.LogError(
                            "Certificate chain could not be verified. Double check that the certificate authority in the secret matches the redis instance");
                    }

                    return caFound;
                }

                return false;
            };
        }
    }
}