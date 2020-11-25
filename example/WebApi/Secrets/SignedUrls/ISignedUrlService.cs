using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Example.WebApi.Secrets.SignedUrls
{
    /// <summary>
    /// A service for generating signed urls.
    /// </summary>
    public interface ISignedUrlService
    {
        /// <summary>
        /// Gets a signed URL for accessing the object.
        /// </summary>
        /// <param name="bucketName">Name of the bucket where the object is stored.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="method">HttpMethod to use for the Url, either GET for reading or PUT for uploading.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Returns a Task with a string result that contains the signed URL.</returns>
        Task<string> GetSignedUrlAsync(
            string bucketName,
            string objectName,
            HttpMethod method,
            CancellationToken cancellationToken);
    }
}