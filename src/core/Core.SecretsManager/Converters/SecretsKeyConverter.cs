using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Core.SecretsManager.Converters
{
    /// <summary>
    /// A converter class for <see cref="SecretsKey"/>.
    /// </summary>
    public static class SecretsKeyConverter
    {
        /// <summary>
        /// Converts <see cref="SecretsOptions"/> to <see cref="SecretsKey"/>.
        /// </summary>
        /// <param name="options">The secret options.</param>
        /// <returns>The immutable secrets key.</returns>
        public static SecretsKey ToSecretsKey(this SecretsOptions options) =>
            new SecretsKey(options.ProjectId, options.SecretId, options.SecretVersionId, options.CacheExpirationInMinutesRelativeToNow);
    }
}