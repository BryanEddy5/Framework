using System.Threading;
using System.Threading.Tasks;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Example.WebApi.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// A controller for testing structured logging of exceptions.
    /// </summary>
    [ApiController]
    [Route("exception")]
    public class ExceptionController
    {
        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <param name="complexExceptionMessage">The message to be caught and thrown. </param>
        /// <param name="logger">The logger. </param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        /// <exception cref="MessageAppException">Throws le exception.</exception>
        [HttpPost]
        public Task ThrowException(
            ComplexExceptionMessage complexExceptionMessage,
            [FromServices] ILogger<ExceptionController> logger,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Logged {@Request}", complexExceptionMessage);

            throw new MessageAppException("Some message to my consumer", "Thrown {@Request}", complexExceptionMessage);
        }
    }
}