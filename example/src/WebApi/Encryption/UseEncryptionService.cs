using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Encryption;

namespace HumanaEdge.Webcore.Example.WebApi.Encryption
{
    /// <summary>
    /// An example class for utilizing <see cref="IEncryptionService"/>.
    /// </summary>
    public class UseEncryptionService
    {
        private readonly IEncryptionService _encryptionService;

        /// <summary>
        /// Inject <see cref="IEncryptionService"/>.
        /// </summary>
        /// <param name="encryptionService">DI the service to be leveraged.</param>
        public UseEncryptionService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Executes the Bar encryption service to demonstrate how to encrypt and decrypt strings.
        /// </summary>
        /// <param name="bar">The fake input to be Encrypted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        public async Task Execute(string bar, CancellationToken cancellationToken)
        {
            var encryptedString = await _encryptionService.EncryptSymmetric(bar, cancellationToken);

            var decryptedString = await _encryptionService.DecryptSymmetric(bar, cancellationToken);
        }
    }
}