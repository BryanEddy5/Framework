using System.Net.Security;

namespace HumanaEdge.Webcore.Framework.Caching.Services
{
    /// <summary>
    /// A factory for generating a certification validator.
    /// </summary>
    internal interface ICertificateValidationFactory
    {
        /// <summary>
        /// Creates the <see cref="RemoteCertificateValidationCallback"/> for validating the certificate.
        /// </summary>
        /// <returns>The validator.</returns>
        RemoteCertificateValidationCallback Create();
    }
}