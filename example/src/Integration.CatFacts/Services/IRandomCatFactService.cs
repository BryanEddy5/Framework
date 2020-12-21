using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Example.Integration.CatFacts.Client.Contracts;
using HumanaEdge.Webcore.Example.Models.Immutable;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Services
{
    /// <summary>
    /// A service that retrieves random cat facts.
    /// </summary>
    public interface IRandomCatFactService
    {
        /// <summary>
        /// Retrieves a random cat fact.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A random cat fact of the day.</returns>
        Task<CatFact?> GetAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Creates a random cat fact.
        /// </summary>
        /// <param name="randomCatFactRequest">The request contract for creating a random cat fact.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        Task PostAsync(RandomCatFactRequest randomCatFactRequest, CancellationToken cancellationToken);
    }
}