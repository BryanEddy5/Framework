using System.Threading.Tasks;
using HumanaEdge.Webcore.Example.Integration.Calculator.Services;
using HumanaEdge.Webcore.Example.WebApi.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// A collection of endpoints governing the SOAPey calculator integration.
    /// </summary>
    [ApiController]
    [Route("calculate")]
    public class CalculateController
    {
        /// <inheritdoc cref="ICalculatorService"/>
        private readonly ICalculatorService _calculatorService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="calculatorService">The calculator service for SOAPey calculations.</param>
        public CalculateController(ICalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        /// <summary>
        /// The calculate method- performs SOAPey calculations.
        /// </summary>
        /// <param name="calculatorRequest">The incoming request.</param>
        /// <returns>The sum.</returns>
        [HttpPost]
        public async Task<CalculatorResponse> Calculate(CalculatorRequest calculatorRequest)
        {
            var response = await _calculatorService.AddAsync(calculatorRequest.Num1, calculatorRequest.Num2);
            return new CalculatorResponse(response);
        }
    }
}