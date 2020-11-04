using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Core.SecretsManager.Contracts
{
    /// <summary>
    /// The json Key for a service account.
    /// </summary>
    public class ServiceAccountKey : ISecret
    {
        /// <summary>
        /// The type of json key.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// The project id.
        /// </summary>
        [JsonProperty("project_id")]
        public string? ProjectId { get; set; }

        /// <summary>
        /// The private key id.
        /// </summary>
        [JsonProperty("private_key_id")]
        public string? PrivateKeyId { get; set; }

        /// <summary>
        /// The actual private key value.
        /// </summary>
        [JsonProperty("private_key")]
        public string? PrivateKey { get; set; }

        /// <summary>
        /// The service account email.
        /// </summary>
        [JsonProperty("client_email")]
        public string? ClientEmail { get; set; }

        /// <summary>
        /// The service account unique id.
        /// </summary>
        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        /// <summary>
        /// The uri for authentication.
        /// </summary>
        [JsonProperty("auth_uri")]
        public string? AuthUri { get; set; }

        /// <summary>
        /// The uri for retrieving a token.
        /// </summary>
        [JsonProperty("token_uri")]
        public string? TokenUri { get; set; }

        /// <summary>
        /// The certificate url.
        /// </summary>
        [JsonProperty("auth_provider_x509_cert_url")]
        public string? AuthProviderX509CertUrl { get; set; }

        /// <summary>
        /// The service url for the service account.
        /// </summary>
        [JsonProperty("client_x509_cert_url")]
        public string? ClientX509CertUrl { get; set; }
    }
}