using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using HumanaEdge.Webcore.Core.Common.Serialization;
using HumanaEdge.Webcore.Framework.PubSub.TraceContext;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.PubSub.Publication
{
    /// <inheritdoc />
    internal sealed class PublishRequestConverter : IPublishRequestConverter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishRequestConverter"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Accesses the http context.</param>
        public PublishRequestConverter(IHttpContextAccessor httpContextAccessor)
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
                    { TracingKeys.RequestId, _httpContextAccessor?.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString() },
                    { TracingKeys.TraceId, Activity.Current?.RootId ?? Guid.NewGuid().ToString() },
                    { TracingKeys.SpanId, Activity.Current?.SpanId.ToString() ! },
                    { TracingKeys.ParentId, Activity.Current?.ParentId ! },
                    { TracingKeys.TraceParent, Activity.Current?.Id ! }
                }.Where(x => !string.IsNullOrEmpty(x.Value))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}