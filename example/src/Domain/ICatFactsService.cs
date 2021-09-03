using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Example.Models.Immutable;

namespace HumanaEdge.Webcore.Domain
{
    /// <summary>
    /// A service for retrieving cat facts.
    /// </summary>
    public interface ICatFactsService
    {
        /// <summary>
        /// Retrieves a random cat fact.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="forceRefresh">Forces the cache to refresh. </param>
        /// <returns>A random cat fact of the day.</returns>
        Task<CatFact?> GetAsync(CancellationToken cancellationToken, bool forceRefresh = false);
    }
}