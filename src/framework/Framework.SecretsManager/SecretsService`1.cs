using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.SecretsManager;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using HumanaEdge.Webcore.Core.SecretsManager.Converters;
using HumanaEdge.Webcore.Framework.SecretsManager.Clients;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Framework.SecretsManager
{
    /// <inheritdoc />
    internal sealed class SecretsService<TSecret> : ISecretsService<TSecret>
        where TSecret : ISecret
    {
        private readonly ISecretsHandler _handler;

        private readonly IOptionsMonitor<SecretsOptions> _options;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="handler">The client to retrieve the secret.</param>
        /// <param name="options">The configuration options required for retrieving the secret.</param>
        public SecretsService(ISecretsHandler handler, IOptionsMonitor<SecretsOptions> options)
        {
            _handler = handler;
            _options = options;
        }

        /// <inheritdoc />
        public async Task<TSecret> GetAsync(CancellationToken cancellationToken) =>
            await _handler.GetAsync<TSecret>(_options.Get(typeof(TSecret).Name).ToSecretsKey(), cancellationToken);
    }
}