using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace HumanaEdge.Webcore.Framework.Caching.Services
{
    /// <inheritdoc />
    internal sealed class CertificateValidationFactory
        : ICertificateValidationFactory
    {
        private readonly ICertificateAuthorityService _certificateAuthorityService;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="certificateAuthorityService">A service for getting the certificate.</param>
        public CertificateValidationFactory(ICertificateAuthorityService certificateAuthorityService)
        {
            _certificateAuthorityService = certificateAuthorityService;
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
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    var certificateAuthority = _certificateAuthorityService.GetCertificate();

                    var redisCertificateAuthority = new X509Certificate2(certificateAuthority);

                    var caFound = chain != null &&
                                  chain.ChainElements
                                      .Cast<X509ChainElement>()
                                      .Any(x => x.Certificate.Thumbprint == redisCertificateAuthority.Thumbprint);

                    return caFound;
                }

                return false;
            };
        }
    }
}