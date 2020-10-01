using System.Text.RegularExpressions;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// An enumeration of supported media types that can be sent / received by REST clients.
    /// </summary>
    public sealed class MediaType
    {
        /// <summary>
        /// JSON data.
        /// </summary>
        public static readonly MediaType Json = new MediaType(
            nameof(Json),
            "application/json",
            "(application|text)\\/(json|([a-z]+)\\+json)");

        /// <summary>
        /// XML data.
        /// </summary>
        public static readonly MediaType Xml = new MediaType(
            nameof(Xml),
            "application/xml",
            "(application|text)\\/(xml|([a-z]+)\\+xml)");

        /// <summary>
        /// FormUrlEncoded data.
        /// </summary>
        public static readonly MediaType FormUrlEncoded = new MediaType(
            nameof(FormUrlEncoded),
            "application/x-www-form-urlencoded",
            "application\\/x-www-form-urlencoded");

        private MediaType(string name, string mimeType, string mimeTypeRegex)
        {
            Name = name;
            MimeType = mimeType;
            MimeTypeRegexTest = new Regex(mimeTypeRegex, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// The official MIME type associated with the media type.
        /// </summary>
        public string MimeType { get; }

        /// <summary>
        /// A regular expression which can be used to test a given content type header to determine if the content-type can be
        /// treated as this instance of <see cref="MediaType" />.
        /// </summary>
        public Regex MimeTypeRegexTest { get; }

        /// <summary>
        /// The friendly name assigned to the media type.
        /// </summary>
        public string Name { get; }
    }
}