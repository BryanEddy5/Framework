using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using HumanaEdge.Webcore.Core.Common.Validators;
using HumanaEdge.Webcore.Core.SecretsManager;
using HumanaEdge.Webcore.Core.SecretsManager.Contracts;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Example.WebApi.Secrets.SignedUrls
{
    /// <inheritdoc />
    internal sealed class SignedUrlService : ISignedUrlService
    {
        private readonly IOptionsSnapshot<SignedUrlOptions> _config;

        private readonly ISecretsService<ServiceAccountKey> _secretsClient;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="config">The configuration settings.</param>
        /// <param name="secretsClient">A service for retrieving a service account.</param>
        public SignedUrlService(
            IOptionsSnapshot<SignedUrlOptions> config,
            ISecretsService<ServiceAccountKey> secretsClient)
        {
            _config = config;
            _secretsClient = secretsClient;
        }

        /// <inheritdoc />
        public async Task<string> GetSignedUrlAsync(
            string bucketName,
            string objectName,
            HttpMethod method,
            CancellationToken cancellationToken)
        {
            bucketName.AssertNotNullOrEmpty();
            objectName.AssertNotNullOrEmpty();

            var initializer = new ServiceAccountCredential.Initializer(_config.Value.ServiceAccount)
                .FromPrivateKey((await _secretsClient.GetAsync(cancellationToken)).PrivateKey);
            var serviceAccountCredential = new ServiceAccountCredential(initializer);
            var url = await UrlSigner
                .FromServiceAccountCredential(serviceAccountCredential)
                .SignAsync(
                    bucketName,
                    objectName,
                    TimeSpan.FromHours(_config.Value.SignedUrlDurationInHours),
                    method,
                    cancellationToken: cancellationToken);

            return url;
        }
    }
}