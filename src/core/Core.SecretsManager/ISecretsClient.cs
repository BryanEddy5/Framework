using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Core.SecretsManager
{
    /// <summary>
    /// A client for accessing stored secrets.
    /// </summary>
    public interface ISecretsClient
    {
        /// <summary>
        /// Retrieves a secret based on the secret configuration options.
        /// </summary>
        /// <param name="secretsOptions">The secret request for retrieving a secret payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="TSecret">The secret's object shape.</typeparam>
        /// <returns>The secret object.</returns>
        public Task<TSecret> GetAsync<TSecret>(SecretsKey secretsOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the byte content of a secret.
        /// </summary>
        /// <param name="secretsOptions">The secret request for retrieving a secret payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An array of bytes of the secret.</returns>
        Task<byte[]> GetBytesAsync(SecretsOptions secretsOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a secret based on the secret configuration options.
        /// </summary>
        /// <param name="secretsOptions">The secret request for retrieving a secret payload.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A stream of the secret.</returns>
        public Task<Stream> GetAsync(SecretsOptions secretsOptions, CancellationToken cancellationToken);
    }
}