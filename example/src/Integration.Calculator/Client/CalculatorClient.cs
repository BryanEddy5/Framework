using System;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Client.Factory;
using Microsoft.Extensions.Options;

namespace HumanaEdge.Webcore.Example.Integration.Calculator.Client
{
    /// <inheritdoc cref="ICalculatorClient"/>
    [DiComponent(Target = typeof(ICalculatorClient))]
    internal sealed class CalculatorClient
        : BaseSoapClient<CalculatorClient, CalculatorSoap>, ICalculatorClient
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="behaviorFactory">The factory for generating endpoint behaviors.</param>
        /// <param name="options">The configuration for this client.</param>
        public CalculatorClient(
            IEndpointBehaviorFactory behaviorFactory,
            IOptions<CalculatorClientOptions> options)
            : base(behaviorFactory, ConfigureOptions(options.Value))
        {
        }

        /// <inheritdoc />
        public async Task<int> AddAsync(int num1, int num2)
        {
            return await Channel.AddAsync(num1, num2);
        }

        /// <summary>
        /// Builds the configuration associated with this client.
        /// </summary>
        /// <param name="options">The configuration for this client.</param>
        /// <returns>The built configuration for the client.</returns>
        private static SoapClientOptions ConfigureOptions(CalculatorClientOptions options)
        {
            return new SoapClientOptions.Builder(options.BaseEndpoint)
                .ConfigureTimeout(TimeSpan.FromMilliseconds(options.TimeoutMilliseconds))
                .ConfigureHeader("x-jeremys-secret", "his-SOAP-is-just-bleach")
                .Build();
        }
    }
}