using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using HumanaEdge.Webcore.Core.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <summary>
    /// Media type formatter for form-urlencoded MIME types.
    /// </summary>
    internal sealed class FormUrlEncodedMediaTypeFormatter : IMediaTypeFormatter
    {
        /// <inheritdoc />
        public MediaType[] MediaTypes { get; } = { MediaType.FormUrlEncoded };

        /// <inheritdoc />
        public bool TryFormat<T>(
            MediaType mediaType,
            IRestFormattingSettings settings,
            T data,
            out HttpContent httpContent)
        {
            var serializer = JsonSerializer.Create(settings.JsonSerializerSettings);
            var jObject = JObject.FromObject(data!, serializer).ToObject<Dictionary<string, string>>();
            httpContent = new FormUrlEncodedContent(jObject);

            return true;
        }

        /// <inheritdoc/>
        public bool TryParse<T>(
            byte[] bytes,
            IRestFormattingSettings formattingSettings,
            MediaTypeHeaderValue mediaTypeHeaderValue,
            out T obj)
        {
            throw new NotImplementedException("No support to parsing FormUrlEncoded content at this time");
        }
    }
}