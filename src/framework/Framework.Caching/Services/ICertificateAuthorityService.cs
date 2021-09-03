namespace HumanaEdge.Webcore.Framework.Caching.Services
{
    /// <summary>
    /// Retrieves the certificate authority.
    /// </summary>
    internal interface ICertificateAuthorityService
    {
        /// <summary>
        /// Retrieves the certificate authority.
        /// </summary>
        /// <returns>The base 64 encoded cert.</returns>
        string GetCertificate();
    }
}