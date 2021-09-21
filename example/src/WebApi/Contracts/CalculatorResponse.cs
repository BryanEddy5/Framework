namespace HumanaEdge.Webcore.Example.WebApi.Contracts
{
    /// <summary>
    /// A response from the SOAPey calculator service.
    /// </summary>
    public sealed class CalculatorResponse
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="answer">The answer to the calculation.</param>
        public CalculatorResponse(int answer)
        {
            Answer = answer;
        }

        /// <summary>
        /// The answer to the calculation requested.
        /// </summary>
        public int Answer { get; }
    }
}