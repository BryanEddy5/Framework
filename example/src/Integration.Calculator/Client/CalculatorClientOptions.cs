using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Example.Integration.Calculator.Client
{
    /// <summary>
    /// This SOAPey client's configuration.
    /// </summary>
    [DiOptions]
    public class CalculatorClientOptions
    {
        /// <summary>
        /// The base uri used by the SOAP client.
        /// </summary>
        public string BaseEndpoint { get; set; } = null!;

        /// <summary>
        /// The timeout in milliseconds for this client.
        /// </summary>
        public int TimeoutMilliseconds { get; set; } = 15000;

        /// <summary>
        /// The maximum number of times this client will retry.
        /// </summary>
        public int MaxRetryCount { get; set; } = 6;
    }
}