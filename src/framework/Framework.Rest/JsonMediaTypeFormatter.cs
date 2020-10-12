using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using HumanaEdge.Webcore.Core.Rest;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <summary>
    /// Media type formatter for JSON-based MIME types.
    /// </summary>
    internal sealed class JsonMediaTypeFormatter : IMediaTypeFormatter
    {
        /// <inheritdoc/>
        public MediaType[] MediaTypes { get; } = { MediaType.Json };

        /// <inheritdoc />
        public bool TryFormat<T>(
            MediaType mediaType,
            IRestFormattingSettings restFormattingSettings,
            T data,
            out HttpContent? httpContent)
        {
            if (!MediaTypes.Contains(mediaType))
            {
                httpContent = null;
                return false;
            }

            var json = JsonConvert.SerializeObject(data, restFormattingSettings.JsonSerializerSettings);
            httpContent = new StringContent(json, Encoding.UTF8, MediaType.Json.MimeType);
            return true;
        }

        /// <inheritdoc />
        public bool TryParse<T>(
            byte[] bytes,
            IRestFormattingSettings formattingSettings,
            MediaTypeHeaderValue mediaTypeHeaderValue,
            out T obj)
        {
            var encodingName = mediaTypeHeaderValue.CharSet ?? Encoding.UTF8.WebName;
            var encoding = Encoding.GetEncoding(encodingName);

            var json = encoding.GetString(bytes);
            obj = JsonConvert.DeserializeObject<T>(json, formattingSettings.JsonSerializerSettings) !;

            return true;
        }
    }
}