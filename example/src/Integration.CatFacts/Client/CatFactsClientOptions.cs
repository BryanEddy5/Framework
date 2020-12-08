using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Example.Integration.CatFacts.Client
{
    /// <summary>
    ///     Configuration options for the <see cref="ICatFactsClient" />.
    /// </summary>
    [DiOptions]
    public class CatFactsClientOptions
    {
        /// <summary>
        ///     Configuration key used to access to the base URI for Cat Facts services.
        /// </summary>
        public string BaseUri { get; set; } = null!;

        /// <summary>
        ///     The resiliency configuration settings for the client.
        /// </summary>
        public Resiliency Resilience { get; set; } = new Resiliency();

        /// <summary>
        ///     Optional. Default Cat Facts request timeout, in milliseconds.
        /// </summary>
        public int TimeoutMilliseconds { get; set; } = 1000;

        /// <summary>
        /// Api key configuration settings.
        /// </summary>
        public ApiKeyOptions ApiKey { get; set; } = new ApiKeyOptions();

        /// <summary>
        ///     The resiliency configuration settings for the client.
        /// </summary>
        public class Resiliency
        {
            /// <summary>
            ///     The number of retries for a resiliency policy.
            /// </summary>
            public int RetryAttempts { get; set; } = 4;
        }

        /// <summary>
        /// Authentication credentials required for http requests.
        /// </summary>
        public class ApiKeyOptions
        {
            /// <summary>
            /// The api key header key.
            /// </summary>
            public string HeaderKey { get; set; } = "x-api-key";

            /// <summary>
            /// The api key header value.
            /// </summary>
            public string HeaderValue { get; set; } = null!; // Set sensitive information in the CI/CD secret variable.
        }
    }
}