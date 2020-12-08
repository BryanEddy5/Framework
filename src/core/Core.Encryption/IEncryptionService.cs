using System.Threading;
using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Core.Encryption
{
    /// <summary>
    /// Provides the ability to encrypt and decrypt text.
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Encrypts data.
        /// </summary>
        /// <param name="message">The string that needs to be encrypted.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The encrypted string.</returns>
        Task<string> EncryptSymmetric(string message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Decrypts data.
        /// </summary>
        /// <param name="cipherText">The string that is the encrypted text.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The decrypted string.</returns>
        Task<string> DecryptSymmetric(string cipherText, CancellationToken cancellationToken = default);
    }
}