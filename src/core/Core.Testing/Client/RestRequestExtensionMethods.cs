using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace HumanaEdge.Webcore.Core.Testing.Client
{
    /// <summary>
    /// <see cref="IRestRequest"/> extension methods.
    /// </summary>
    public static class RestRequestExtensionMethods
    {
        /// <summary>
        /// Adds serializes a request and adds it to the header.
        /// </summary>
        /// <param name="restRequest">The rest request the header will be added to.</param>
        /// <param name="headerKey">The header key.</param>
        /// <param name="headerValue">The header value that will be serialized.</param>
        /// <typeparam name="T">The header value shape.</typeparam>
        /// <returns>The same rest request for fluent chaining.</returns>
        public static IRestRequest AddJsonHeader<T>(this IRestRequest restRequest, string headerKey, T headerValue)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver =
                    new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy(true, false) },
            };

            var json = JsonConvert.SerializeObject(headerValue, jsonSettings);
            return restRequest.AddHeader(headerKey, json);
        }
    }
}