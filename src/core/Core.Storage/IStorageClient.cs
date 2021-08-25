using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.Storage
{
    /// <summary>
    /// A client for interacting with GCP storage.
    /// </summary>
    public interface IStorageClient
    {
        /// <summary>
        /// Method to upload files using <see cref="IStorageClient"/>.
        /// </summary>
        /// <param name="filename"> The filename to upload. </param>
        /// <param name="contentType"> The content type. </param>
        /// <param name="stream"> The file contents to upload. </param>
        /// <param name="options"> Options containing bucket and project names. </param>
        /// <param name="cancellationToken"> A cancellation token. </param>
        /// <returns> An awaitable task. </returns>
        Task UploadObjectAsync(
            string filename,
            string contentType,
            Stream stream,
            CloudStorageOptions options,
            CancellationToken cancellationToken);
    }
}