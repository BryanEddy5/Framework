using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using HumanaEdge.Webcore.Core.Storage;

namespace HumanaEdge.Webcore.Framework.Storage
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    internal sealed class GcpStorageClient
        : IStorageClient
    {
        private static StorageClient? _storageClient;

        /// <inheritdoc />
        public async Task UploadObjectAsync(
            string filename,
            string contentType,
            Stream stream,
            CloudStorageOptions options,
            CancellationToken cancellationToken)
        {
            _storageClient ??= await StorageClient.CreateAsync();
            _ = await _storageClient.UploadObjectAsync(
                options.GcpBucket,
                filename,
                contentType,
                stream,
                new UploadObjectOptions { UserProject = options.GcpProject },
                cancellationToken);
        }
    }
}