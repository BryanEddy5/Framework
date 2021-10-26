namespace HumanaEdge.Webcore.Core.SecretsManager.Contracts
{
    /// <summary>
    /// A request for retrieving a secret.
    /// </summary>
    public class SecretsOptions
    {
        /// <summary>
        /// The ProjectId where the secrets manager is located.
        /// </summary>
        public string ProjectId { get; set; } = null!;

        /// <summary>
        /// The unique identifier for the secret.
        /// </summary>
        public string SecretId { get; set; } = null!;

        /// <summary>
        /// The version of the secret.
        /// </summary>
        public string SecretVersionId { get; set; } = null!;

        /// <summary>
        /// The absolute cache expiration relative to now.
        /// </summary>
        public int CacheExpirationInMinutesRelativeToNow { get; set; } = 1440;
    }
}