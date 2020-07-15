namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    /// A factory pattern for generating a <see cref="IRestClient" />.
    /// </summary>
    public interface IRestClientFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IRestClient" /> with the given options set.
        /// </summary>
        /// <typeparam name="TRestClient">The type of the calling class, used to assign a type-safe name to the client instance.</typeparam>
        /// <param name="options">Options for the client configuration.</param>
        /// <returns>An instance of <see cref="IRestClient" />.</returns>
        IRestClient CreateClient<TRestClient>(RestClientOptions options);
    }
}