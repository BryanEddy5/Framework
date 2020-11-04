using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Clients
{
    /// <inheritdoc />
    internal sealed class SecretsHandler : ISecretsHandler
    {
        /// <summary>
        /// Cache for the retrieved secrets.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ISecret> _cache =
            new ConcurrentDictionary<Type, ISecret>();

        /// <summary>
        /// A client for retrieving the stored secret.
        /// </summary>
        private readonly IInternalSecretsClient _internalSecretsClient;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="internalSecretsClient">The client for retrieving secrets.</param>
        public SecretsHandler(IInternalSecretsClient internalSecretsClient)
        {
            _internalSecretsClient = internalSecretsClient;
        }

        /// <inheritdoc />
        public async Task<TSecret> GetAsync<TSecret>(SecretsKey secretsKey, CancellationToken cancellationToken)
            where TSecret : ISecret
        {
            if (_cache.TryGetValue(typeof(TSecret), out var secret))
            {
                return (TSecret)secret;
            }

            secret = await _internalSecretsClient.GetAsync<TSecret>(secretsKey, cancellationToken);
            _cache.TryAdd(typeof(TSecret), secret);

            return (TSecret)secret;
        }
    }
}