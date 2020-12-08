using System;
using System.Diagnostics.CodeAnalysis;

namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// The payload of a bucket subscription.
    /// Link to object shape and documentation https://cloud.google.com/storage/docs/json_api/v1/objects#resource-representations .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BucketMetadata
    {
        /// <summary>
        /// The name of the bucket containing this object.
        /// </summary>
        public string? Bucket { get; set; }

        /// <summary>
        /// Content-Type of the object data. If an object is stored without a Content-Type, it is served as
        /// application/octet-stream.
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// HTTP 1.1 Entity tag for the object.
        /// </summary>
        public string? Etag { get; set; }

        /// <summary>
        /// The content generation of this object. Used for object versioning.
        /// </summary>
        public string? Generation { get; set; }

        /// <summary>
        /// The ID of the object, including the bucket name, object name, and generation number.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// he kind of item this is. For objects, this is always storage#object.
        /// </summary>
        public string? Kind { get; set; }

        /// <summary>
        /// MD5 hash of the data; encoded using base64. For more information about using the MD5 hash, see Hashes and ETags: Best
        /// Practices.
        /// </summary>
        public string? Md5Hash { get; set; }

        /// <summary>
        /// Media download link.
        /// </summary>
        public string? MediaLink { get; set; }

        /// <summary>
        /// The version of the metadata for this object at this generation. Used for preconditions and for detecting changes in
        /// metadata. A metageneration number is only meaningful in the context of a particular generation of a particular object.
        /// </summary>
        public string? Metageneration { get; set; }

        /// <summary>
        /// The name of the object. Required if not specified by URL parameter.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The link to this object.
        /// </summary>
        public string? SelfLink { get; set; }

        /// <summary>
        /// Content-Length of the data in bytes.
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Storage class of the object.
        /// </summary>
        public string? StorageClass { get; set; }

        /// <summary>
        /// The creation time of the object in RFC 3339 format.
        /// </summary>
        public DateTimeOffset TimeCreated { get; set; }

        /// <summary>
        /// The time at which the object's storage class was last changed. When the object is initially created, it will be set to
        /// timeCreated.
        /// </summary>
        public DateTimeOffset TimeStorageClassUpdated { get; set; }

        /// <summary>
        /// The modification time of the object metadata in RFC 3339 format.
        /// </summary>
        public DateTimeOffset Updated { get; set; }
    }
}