using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// Serializes and deserializes http requests base on the content type.
    /// </summary>
    public interface IMediaTypeFormatter
    {
        /// <summary>
        /// The <see cref="MediaType" />  that is associated with this formatter.
        /// </summary>
        MediaType[] MediaTypes { get; }

        /// <summary>
        /// Formats an outgoing http request to generate <see cref="HttpContent" /> to be used in http requests.
        /// </summary>
        /// <param name="mediaType">Indicates the media type to be formatted to.</param>
        /// <param name="restFormattingSettings">Settings for tailoring the formatting of the request.</param>
        /// <param name="data">The <see cref="Type" /> to be serialized.</param>
        /// <param name="httpContent">The response body of the outgoing request.</param>
        /// <typeparam name="T">The <see cref="Type" /> of the object that is being serialized.</typeparam>
        /// <returns>Indicates the success of the formatting.</returns>
        public bool TryFormat<T>(
            MediaType mediaType,
            IRestFormattingSettings restFormattingSettings,
            T data,
            out HttpContent? httpContent);

        /// <summary>
        /// Parses an http response and converts it to a type of {T}
        /// while also reporting if the parsing was successful.
        /// </summary>
        /// <param name="bytes">The response bytes from the response.</param>
        /// <param name="formattingSettings">Customizable settings for parsing the response.</param>
        /// <param name="mediaTypeHeaderValue">The Content-Type header value from the response.</param>
        /// <param name="obj">The object created from the serialized request response.</param>
        /// <typeparam name="T">The <see cref="Type" /> to be parsed to.</typeparam>
        /// <returns>Indicates if the parsing was successful.</returns>
        bool TryParse<T>(
            byte[] bytes,
            IRestFormattingSettings formattingSettings,
            MediaTypeHeaderValue mediaTypeHeaderValue,
            out T obj);
    }
}