using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Clients
{
    /// <summary>
    /// A client for accessing stored secrets.
    /// </summary>
    internal interface IInternalSecretsClient
    {
        /// <summary>
        /// Retrieves a secret based on the secret configuration options.
        /// </summary>
        /// <param name="secretsOptions">The secret request for retrieving a secret payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TSecret">The secret's object shape.</typeparam>
        /// <returns>The secret object.</returns>
        public Task<TSecret> GetAsync<TSecret>(SecretsKey secretsOptions, CancellationToken cancellationToken);
    }
}