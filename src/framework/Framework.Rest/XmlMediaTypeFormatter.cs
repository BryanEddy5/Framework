using System.Net.Http;
using System.Net.Http.Headers;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Core.Rest;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <summary>
    ///     Media type formatter for XML-based MIME-types.
    /// </summary>
    internal sealed class XmlMediaTypeFormatter : IMediaTypeFormatter
    {
        /// <inheritdoc />
        public MediaType[] MediaTypes { get; } = { MediaType.Xml };

        /// <inheritdoc />
        public bool TryFormat<T>(
            MediaType mediaType,
            IRestFormattingSettings settings,
            T data,
            out HttpContent httpContent)
        {
            var bytes = XmlSerializer.SerializeBytes(data);
            httpContent = new ByteArrayContent(bytes);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse($"${MediaType.Xml.MimeType}; charset=utf-8");
            return true;
        }

        /// <inheritdoc />
        public bool TryParse<T>(
            byte[] bytes,
            IRestFormattingSettings formattingSettings,
            MediaTypeHeaderValue mediaTypeHeaderValue,
            out T obj)
        {
            obj = XmlSerializer.DeserializeBytes<T>(bytes);
            return true;
        }
    }
}