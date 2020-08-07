using System.IO;
using System.Net;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// A response class for representing file responses.
    /// </summary>
    public class FileResponse : BaseRestResponse
    {
        /// <summary>
        /// Ctor for a FileResponse. This class should be utilized whenever the Rest Client is downloading a file via HTTP, as it uses
        /// Streams to parse the response body as opposed to loading the response body all at once.
        /// </summary>
        /// <param name="isSuccessful">Whether or not the response was successful.</param>
        /// <param name="fileStream">The stream corresponding to the response body.</param>
        /// <param name="httpStatusCode">The status code for the response.</param>
        /// <param name="contentType">The string value of the Content-Type response header.</param>
        /// <param name="fileName">The name of the file based on the Content-Disposition header.</param>
        public FileResponse(bool isSuccessful, Stream fileStream, HttpStatusCode httpStatusCode, string? contentType, string? fileName)
        : base(isSuccessful, httpStatusCode)
        {
            FileStream = fileStream;
            ContentType = contentType;
            FileName = fileName;
        }

        /// <summary>
        /// The stream corresponding to the contents of the file.
        /// </summary>
        public Stream FileStream { get; }

        /// <summary>
        /// The string representation of the Content-Type header value.
        /// </summary>
        public string? ContentType { get; }

        /// <summary>
        /// The name of the file based on the ContentDisposition header value.
        /// </summary>
        public string? FileName { get; }
    }
}