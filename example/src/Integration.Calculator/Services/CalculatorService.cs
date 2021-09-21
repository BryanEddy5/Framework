using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.DependencyInjection;
using HumanaEdge.Webcore.Example.Integration.Calculator.Client;

namespace HumanaEdge.Webcore.Example.Integration.Calculator.Services
{
    /// <inheritdoc />
    [DiComponent]
    internal sealed class CalculatorService : ICalculatorService
    {
        /// <inheritdoc cref="ICalculatorClient"/>
        private readonly ICalculatorClient _client;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="calculatorClient">The client for the SOAPey calculator.</param>
        public CalculatorService(ICalculatorClient calculatorClient)
        {
            _client = calculatorClient;
        }

        /// <inheritdoc />
        public async Task<int> AddAsync(int num1, int num2)
        {
            var result = await _client.AddAsync(num1, num2);
            return result;
        }
    }
}