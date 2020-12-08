using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Example.WebApi.Secrets.SignedUrls
{
    /// <summary>
    /// Configuration settings for a signed url.
    /// </summary>
    [DiOptions]
    public class SignedUrlOptions
    {
        /// <summary>
        /// The service account for used for signed urls.
        /// </summary>
        public string? ServiceAccount { get; set; }

        /// <summary>
        /// The lifetime of a signed url in hours.
        /// </summary>
        public int SignedUrlDurationInHours { get; set; }
    }
}