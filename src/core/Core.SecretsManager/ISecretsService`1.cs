using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.SecretsManager
{
    /// <summary>
    /// A service for retrieving a particular secret.
    /// </summary>
    /// <typeparam name="TSecret">The secret's shape.</typeparam>
    public interface ISecretsService<TSecret>
    {
        /// <summary>
        /// Retrieves the secret.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The secret.</returns>
        Task<TSecret> GetAsync(CancellationToken cancellationToken);
    }
}