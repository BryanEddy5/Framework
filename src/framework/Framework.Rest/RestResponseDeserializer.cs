using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using HumanaEdge.Webcore.Core.Rest;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <summary>
    /// Deserializes the request body from a http request.
    /// </summary>
    internal sealed class RestResponseDeserializer : IRestResponseDeserializer
    {
        /// <summary>
        /// The formatting settings for deserializing the request body.
        /// </summary>
        private readonly IRestFormattingSettings _formattingSettings;

        /// <summary>
        /// A collection of media type formatters.
        /// </summary>
        private readonly IDictionary<MediaType, IMediaTypeFormatter> _mediaTypeFormatters;

        /// <summary>
        /// The media type from the http response.
        /// </summary>
        private readonly MediaTypeHeaderValue? _mediaTypeHeaderValue;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="mediaTypeFormatters">A collection of media type formatters.</param>
        /// <param name="mediaTypeHeaderValue">The media type from the http response.</param>
        /// <param name="responseBytes">The content of the response body.</param>
        /// <param name="formattingSettings">The formatting settings for deserializing the request body.</param>
        public RestResponseDeserializer(
            IDictionary<MediaType, IMediaTypeFormatter> mediaTypeFormatters,
            MediaTypeHeaderValue? mediaTypeHeaderValue,
            byte[] responseBytes,
            IRestFormattingSettings formattingSettings)
        {
            _mediaTypeFormatters = mediaTypeFormatters;
            _mediaTypeHeaderValue = mediaTypeHeaderValue;
            ResponseBytes = responseBytes;
            _formattingSettings = formattingSettings;
        }

        /// <summary>
        /// The response body payload from the request.
        /// </summary>
        public byte[] ResponseBytes { get; }

        /// <summary>
        /// Converts the http request body to a designated <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type" /> to be deserialized to.</typeparam>
        /// <returns>The designated <see cref="Type" />.</returns>
        public TResponse ConvertTo<TResponse>()
        {
            if (ResponseBytes.Length == 0 || _mediaTypeHeaderValue?.MediaType == null)
            {
                throw new FormatFailedRestException("The response is empty.  Null cannot be formatted.");
            }

            var mediaType = _mediaTypeFormatters.Keys.FirstOrDefault(
                mt => mt.MimeTypeRegexTest.IsMatch(_mediaTypeHeaderValue.MediaType));
            if (mediaType == null)
            {
                throw new FormatFailedRestException(
                    $"Media type {_mediaTypeHeaderValue.MediaType} with encoding {_mediaTypeHeaderValue.CharSet} does not have a supported {nameof(IMediaTypeFormatter)}");
            }

            try
            {
                var mediaTypeFormatter = _mediaTypeFormatters[mediaType];
                var didFormat = mediaTypeFormatter.TryParse(
                    ResponseBytes,
                    _formattingSettings,
                    _mediaTypeHeaderValue,
                    out TResponse obj);
                if (!didFormat)
                {
                    throw new FormatFailedRestException(
                        $"Unable to parse response of media type {_mediaTypeHeaderValue.MediaType} for type {typeof(TResponse).Name} using formatter {nameof(IMediaTypeFormatter)}");
                }

                return obj;
            }
            catch (Exception exception)
            {
                throw new FormatFailedRestException(
                    $"Unable to parse response of media type {_mediaTypeHeaderValue.MediaType} for type {typeof(TResponse).Name} using formatter {nameof(IMediaTypeFormatter)}",
                    exception);
            }
        }
    }
}