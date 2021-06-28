using System;
using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.Rest.AccessTokens
{
    /// <summary>
    /// The service responsible for retrieving a token from either local memory cache or from a given endpoint.
    /// </summary>
    public interface IAccessTokenCacheService
    {
        /// <summary>
        /// Gets the access_token from either cache or the oauth endpoint.
        /// </summary>
        /// <param name="tokenFactory">A function for fetching a new token.</param>
        /// <param name="tokenKey">The unique identifier for the token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="forceRefresh">Forceful refresh by fetching the token from the oauth endpoint.</param>
        /// <returns>The access_token, either from cache or the oauth endpoint.</returns>
        Task<string> GetAsync(
            Func<CancellationToken, Task<string>> tokenFactory,
            string tokenKey,
            CancellationToken cancellationToken,
            bool forceRefresh = false);
    }
}