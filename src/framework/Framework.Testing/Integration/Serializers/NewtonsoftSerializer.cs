using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace HumanaEdge.Webcore.Framework.Testing.Integration.Serializers
{
    /// <summary>
    /// NewtonsoftSerializer class.
    /// </summary>
    internal sealed class NewtonsoftSerializer : ISerializer, IDeserializer
    {
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftSerializer" /> class.
        /// </summary>
        /// <param name="settings"> settings is a parameter of type JsonSerializerSeetings object. </param>
        public NewtonsoftSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public string ContentType
        {
            get => "application/json";
            set { }
        }

        /// <inheritdoc />
        public T Deserialize<T>(IRestResponse response)
        {
            var jsonString = response.Content;

            if (!string.IsNullOrWhiteSpace(response.Request.RootElement))
            {
                jsonString = JObject.Parse(response.Content).SelectToken(response.Request.RootElement)?.ToString();
            }

            return JsonConvert.DeserializeObject<T>(jsonString !, _settings) !;
        }

        /// <summary>
        /// Serialize json object.
        /// </summary>
        /// <param name="obj"> obj of type object. </param>
        /// <returns> Returns the serialized object. </returns>
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _settings);
        }
    }
}