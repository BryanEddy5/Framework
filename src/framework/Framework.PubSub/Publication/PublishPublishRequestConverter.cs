using System;
using System.Collections.Generic;
using System.Diagnostics;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using HumanaEdge.Webcore.Core.Common.Serialization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <inheritdoc />
    internal sealed class PublishPublishRequestConverter : IPublishRequestConverter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishPublishRequestConverter"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Accesses the http context.</param>
        public PublishPublishRequestConverter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public PublishRequest Create<TMessage>(TMessage message, TopicName topicName)
        {
            var json = JsonConvert.SerializeObject(
                message,
                StandardSerializerConfiguration.Settings);
            return new PublishRequest
            {
                Messages =
                {
                    new PubsubMessage
                    {
                        Data = ByteString.CopyFromUtf8(json),
                        Attributes = { GetAttributes() }
                    }
                },
                TopicAsTopicName = topicName,
            };
        }

        private IDictionary<string, string> GetAttributes()
        {
            return new Dictionary<string, string>
            {
                { "RequestId", _httpContextAccessor?.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString() },
            };
        }
    }
}