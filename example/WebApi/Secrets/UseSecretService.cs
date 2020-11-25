using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Core.SecretsManager;

namespace HumanaEdge.Webcore.Example.WebApi.Secrets
{
    /// <summary>
    /// An example of how to use a secret.
    /// </summary>
    [DiComponent]
    public class UseSecretService
    {
        private readonly ISecretsService<FooSecret> _fooSecretsService;

        /// <summary>
        /// Inject <see cref="ISecretsService{TSecret}"/> to be leveraged.
        /// </summary>
        /// <param name="fooSecretsService">The service for retrieving the secret <see cref="FooSecret"/>.</param>
        public UseSecretService(ISecretsService<FooSecret> fooSecretsService)
        {
            _fooSecretsService = fooSecretsService;
        }

        /// <summary>
        /// Pass in a <see cref="CancellationToken"/> or <see cref="CancellationToken.None"/> to retrieve the secret.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var fooSecret = await _fooSecretsService.GetAsync(cancellationToken);
        }
    }
}