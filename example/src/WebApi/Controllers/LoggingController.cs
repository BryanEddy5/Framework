using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// Testing logging.
    /// </summary>
    [Route("/logging")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="logger">yup.</param>
        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GET /logging.
        /// </summary>
        [HttpGet]
        public void Get()
        {
            _logger.LogInformation($"Server starting..");
            _logger.LogCritical(new ArithmeticException("BAD MATH"), "math don't work");
            _logger.LogError(new ApplicationException("AppExceptomp"), "Major Error");
            _logger.LogDebug("Debug! {Basic}", "values");
            _logger.LogWarning("WARNING! {Basic}, {@Object}", "Basicd2354425", new { shazam = "Testing Warning Object 123" });
        }
    }
}