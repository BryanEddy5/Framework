using System.Threading.Tasks;

namespace HumanaEdge.Webcore.Example.Integration.Calculator.Services
{
    /// <summary>
    /// A service that provides the four core math operations.
    /// </summary>
    public interface ICalculatorService
    {
        /// <summary>
        /// Adds two numbers together to get the sum.
        /// </summary>
        /// <param name="num1">The first number.</param>
        /// <param name="num2">The second number.</param>
        /// <returns>The sum of the parameters.</returns>
        Task<int> AddAsync(int num1, int num2);
    }
}