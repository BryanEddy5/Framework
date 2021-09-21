using HumanaEdge.Webcore.Example.Integration.Calculator.Models;

namespace HumanaEdge.Webcore.Example.WebApi.Contracts
{
    /// <summary>
    /// A request to the SOAPey calculator service.
    /// </summary>
    public sealed class CalculatorRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="calculationType">The type of math operation to perform.</param>
        /// <param name="num1">The first number.</param>
        /// <param name="num2">The second number.</param>
        public CalculatorRequest(CalculationType calculationType, int num1, int num2)
        {
            CalculationType = calculationType;
            Num1 = num1;
            Num2 = num2;
        }

        /// <summary>
        /// The type of math operation to perform.
        /// </summary>
        public CalculationType CalculationType { get; }

        /// <summary>
        /// The first number.
        /// </summary>
        public int Num1 { get; }

        /// <summary>
        /// The second number.
        /// </summary>
        public int Num2 { get; }
    }
}