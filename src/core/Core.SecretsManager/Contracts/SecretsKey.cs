namespace HumanaEdge.Webcore.Core.SecretsManager.Contracts
{
    /// <summary>
    /// A request for retrieving a secret.
    /// </summary>
    [Equals(DoNotAddEqualityOperators = true)]
    public class SecretsKey
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="projectId">The ProjectId where the secrets manager is located.</param>
        /// <param name="secretId">The unique identifier for the secret.</param>
        /// <param name="secretVersionId">The version of the secret.</param>
        /// <param name="cacheExpirationInMinutesRelativeToNow">The absolute cache expiration relative to now.</param>
        public SecretsKey(string projectId, string secretId, string secretVersionId, int cacheExpirationInMinutesRelativeToNow)
        {
            ProjectId = projectId;
            SecretId = secretId;
            SecretVersionId = secretVersionId;
            CacheExpirationInMinutesRelativeToNow = cacheExpirationInMinutesRelativeToNow;
        }

        /// <summary>
        /// The ProjectId where the secrets manager is located.
        /// </summary>
        public string ProjectId { get; }

        /// <summary>
        /// The unique identifier for the secret.
        /// </summary>
        public string SecretId { get; }

        /// <summary>
        /// The version of the secret.
        /// </summary>
        public string SecretVersionId { get; }

        /// <summary>
        /// The absolute cache expiration relative to now.
        /// </summary>
        public int CacheExpirationInMinutesRelativeToNow { get; }
    }
}