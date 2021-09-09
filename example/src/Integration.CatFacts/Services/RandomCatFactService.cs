using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.Alerting;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Client;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Client.Contracts;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Converter;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Exceptions;
using HumanaEdge.Webcore.Example.Models.Immutable;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Services
{
    /// <inheritdoc />
    [DiComponent]
    internal sealed class RandomCatFactService : IRandomCatFactService
    {
        /// <summary>
        /// The relative path for the http request.
        /// </summary>
        internal const string RelativePath = "random";

        /// <summary>
        /// A Cat Facts specific client for making RESTful http requests.
        /// </summary>
        private readonly ICatFactsClient _catFactsClient;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="catFactsClient">A Cat Facts specific client for making RESTful http requests.</param>
        public RandomCatFactService(ICatFactsClient catFactsClient)
        {
            _catFactsClient = catFactsClient;
        }

        /// <inheritdoc />
        public async Task<CatFact?> GetAsync(CancellationToken cancellationToken)
        {
            var request = new RestRequest(
                RelativePath,
                HttpMethod.Get);

            var response = await _catFactsClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessful || response.ResponseBytes.Length == 0)
            {
                if (response.StatusCode == HttpStatusCode.NotFound || response.ResponseBytes.Length == 0)
                {
                    throw new NotFoundCatFactsExceptions(RelativePath);
                }

                throw new CatFactsException($"Cat Facts integration failed. HTTP Status Code: {response.StatusCode}");
            }

            return response.ConvertTo<RandomCatFactsResponse>()?.ToCatFact();
        }

        /// <inheritdoc/>
        public async Task PostAsync(RandomCatFactRequest randomCatFactRequest, CancellationToken cancellationToken)
        {
            var request = new RestRequest<RandomCatFactRequest>(
                RelativePath,
                HttpMethod.Post,
                randomCatFactRequest,
                MediaType.Json);
            request.ConfigureAlertCondition(CommonRestAlertConditions.Minimum());

            var response = await _catFactsClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessful)
            {
                throw new CatFactsException($"Cat Facts integration failed. HTTP Status Code: {response.StatusCode}");
            }
        }
    }
}