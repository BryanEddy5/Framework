namespace HumanaEdge.Webcore.Core.Storage
{
    /// <summary>
    /// Options class containing config for working with Google Cloud Storage.
    /// </summary>
    public class CloudStorageOptions
    {
        /// <summary>
        /// The GCP bucket that files are uploaded to.
        /// </summary>
        public string? GcpBucket { get; set; }

        /// <summary>
        /// The GCP project that contains the bucket.
        /// </summary>
        public string? GcpProject { get; set; }
    }
}