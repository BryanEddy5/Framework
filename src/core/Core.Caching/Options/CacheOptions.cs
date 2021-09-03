using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace HumanaEdge.Webcore.Core.Caching.Options
{
    /// <summary>
    /// Cache configuration options.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CacheOptions
    {
        /// <summary>
        /// The connection string to the cache.
        /// </summary>
        [Required]
        public string? ConnectionString { get; set; }

        /// <summary>
        /// The information for getting the self signed certificate authority.
        /// </summary>
        public CertificateAuthorityOptions CertificateAuthority { get; set; } = new CertificateAuthorityOptions();

        /// <summary>
        /// The information for getting the self signed certificate authority.
        /// </summary>
        public class CertificateAuthorityOptions
        {
            /// <summary>
            /// The project Id where the certificate authority is housed.
            /// </summary>
            [Required]
            public string? ProjectId { get; set; }

            /// <summary>
            /// The secret Id where the certificate authority is housed.
            /// </summary>
            [Required]
            public string? SecretId { get; set; }
        }
    }
}