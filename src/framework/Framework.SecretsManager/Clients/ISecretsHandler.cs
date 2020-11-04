using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Clients
{
    /// <summary>
    /// A service for retrieving secrets from Secrets Manager.
    /// </summary>
    internal interface ISecretsHandler
    {
        /// <summary>
        /// Retrieves a secret from Secrets Manager.
        /// </summary>
        /// <param name="secretsKey">The secret request for retrieving a secret.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// /// <typeparam name="TSecret">The object shape of the secret.</typeparam>
        /// <returns>The secret.</returns>
        Task<TSecret> GetAsync<TSecret>(SecretsKey secretsKey, CancellationToken cancellationToken)
            where TSecret : ISecret;
    }
}