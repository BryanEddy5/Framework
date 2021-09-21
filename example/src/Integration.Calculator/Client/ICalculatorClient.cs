using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Example.Integration.Calculator.Client
{
    /// <summary>
    /// A SOAPey client for interacting with the "calculator" service reference.
    /// </summary>
    public interface ICalculatorClient
    {
        /// <summary>
        /// Gets the sum of num1 and num2.
        /// </summary>
        /// <param name="num1">The first number to add.</param>
        /// <param name="num2">The second number to add.</param>
        /// <returns>The sum of the parameters.</returns>
        Task<int> AddAsync(int num1, int num2);
    }
}