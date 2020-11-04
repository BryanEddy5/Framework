﻿namespace HumanaEdge.Webcore.Core.SecretsManager.Contracts
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
        public SecretsKey(string projectId, string secretId, string secretVersionId)
        {
            ProjectId = projectId;
            SecretId = secretId;
            SecretVersionId = secretVersionId;
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
    }
}