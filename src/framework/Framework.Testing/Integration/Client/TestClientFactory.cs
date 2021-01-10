using System;
using System.Linq;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Testing.Client;
using HumanaEdge.Webcore.Core.Testing.Transformations;
using HumanaEdge.Webcore.Framework.Testing.Integration.Serializers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using IRestClient = RestSharp.IRestClient;

namespace HumanaEdge.Webcore.Framework.Testing.Integration.Client
{
    /// <inheritdoc />
    internal sealed class TestClientFactory : ITestClientFactory
    {
        private readonly IAsyncRequestTransformation[] _asyncRequestTransformations;

        private readonly IOptionsMonitor<TestClientOptions> _options;

        private readonly IRequestTransformation[] _requestTransformations;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="options"> Configuration options. </param>
        /// <param name="asyncRequestTransformations"> Asynchronous request transformation services. </param>
        /// <param name="requestTransformations"> Request transformation services. </param>
        public TestClientFactory(
            IOptionsMonitor<TestClientOptions> options,
            IAsyncRequestTransformation[] asyncRequestTransformations,
            IRequestTransformation[] requestTransformations)
        {
            _options = options;
            _asyncRequestTransformations = asyncRequestTransformations;
            _requestTransformations = requestTransformations;
        }

        /// <inheritdoc />
        public ITestClient Create(string name)
        {
            var requestOptions = _options.Get(name) ??
                                 throw new ArgumentException(
                                     $"{name} could not be matched to any configuration key or is null");
            var restClient = new RestClient(new Uri(requestOptions.BaseUrl!));
            CreateRequest(requestOptions, restClient);

            restClient.AddHandler(MediaType.Json.MimeType, () => new NewtonsoftSerializer(GetSettings()));

            var asyncTransformations = _asyncRequestTransformations.Where(x => x.ClientNames.Contains(name)).ToArray();
            var transformations = _requestTransformations.Where(x => x.ClientNames.Contains(name)).ToArray();

            return new TestClient(
                restClient,
                asyncTransformations,
                transformations);
        }

        private IRestClient CreateRequest(TestClientOptions options, IRestClient restClient)
        {
            var headers = options.Headers.ToDictionary(x => x.Key, x => x.Value);
            return restClient.AddDefaultHeaders(headers!);
        }

        private JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver =
                    new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy(true, false) }
            };
        }
    }
}