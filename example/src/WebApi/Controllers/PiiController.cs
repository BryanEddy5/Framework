using Destructurama.Attributed;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HumanaEdge.Webcore.Example.WebApi.Controllers
{
    /// <summary>
    /// Tests PII Logging.
    /// </summary>
    [Route("Pii")]
    [ApiController]
    public class PiiController : ControllerBase
    {
        private readonly ILogger<PiiController> _logger;

        /// <summary>
        /// designated ctor.
        /// </summary>
        /// <param name="logger"><see cref="ILogger"/>.</param>
        public PiiController(ILogger<PiiController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// The controller's Get (and default) method.
        /// </summary>
        [HttpGet]
        public void Get()
        {
            var creditCard = new CreditCard()
            {
                DefaultMasked = "123456789",
                CustomMasked = "123456789",
                ShowFirstThreeThenDefaultMasked = "123456789",
                ShowFirstThreeThenDefaultMaskedPreserveLength = "123456789",
                ShowLastThreeThenDefaultMasked = "123456789",
                ShowLastThreeThenDefaultMaskedPreserveLength = "123456789",
                ShowFirstThreeThenCustomMask = "123456789",
                ShowLastThreeThenCustomMask = "123456789",
                ShowLastThreeThenCustomMaskPreserveLength = "123456789",
                ShowFirstThreeThenCustomMaskPreserveLength = "123456789",
                ShowFirstAndLastThreeAndDefaultMaskInTheMiddle = "123456789",
                ShowFirstAndLastThreeAndCustomMaskInTheMiddle = "123456789",
                ShowFirstAndLastThreeAndCustomMaskInTheMiddle2 = "123456789"
            };

            _logger.LogError("{@creditCard}", creditCard);
        }

        /// <summary>
        /// A sample credit card class to demonstrate Destructurama.Attributed masking.
        /// </summary>
        public class CreditCard
        {
            /// <summary>
            /// 123456789 results in "***".
            /// </summary>
            [LogMasked]
            public string? DefaultMasked { get; set; }

            /// <summary>
            ///  123456789 results in "REMOVED".
            /// </summary>
            [LogMasked(Text = "REMOVED")]
            public string? CustomMasked { get; set; }

            /// <summary>
            ///  123456789 results in "123***".
            /// </summary>
            [LogMasked(ShowFirst = 3)]
            public string? ShowFirstThreeThenDefaultMasked { get; set; }

            /// <summary>
            ///  123456789 results in "123******".
            /// </summary>
            [LogMasked(ShowFirst = 3, PreserveLength = true)]
            public string? ShowFirstThreeThenDefaultMaskedPreserveLength { get; set; }

            /// <summary>
            /// 123456789 results in "***789".
            /// </summary>
            [LogMasked(ShowLast = 3)]
            public string? ShowLastThreeThenDefaultMasked { get; set; }

            /// <summary>
            /// 123456789 results in "******789".
            /// </summary>
            [LogMasked(ShowLast = 3, PreserveLength = true)]
            public string? ShowLastThreeThenDefaultMaskedPreserveLength { get; set; }

            /// <summary>
            ///  123456789 results in "123REMOVED".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowFirst = 3)]
            public string? ShowFirstThreeThenCustomMask { get; set; }

            /// <summary>
            ///  123456789 results in "REMOVED789".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowLast = 3)]
            public string? ShowLastThreeThenCustomMask { get; set; }

            /// <summary>
            ///  123456789 results in "******789".
            /// </summary>
            [LogMasked(ShowLast = 3, PreserveLength = true)]
            public string? ShowLastThreeThenCustomMaskPreserveLength { get; set; }

            /// <summary>
            ///  123456789 results in "123******".
            /// </summary>
            [LogMasked(ShowFirst = 3, PreserveLength = true)]
            public string? ShowFirstThreeThenCustomMaskPreserveLength { get; set; }

            /// <summary>
            /// 123456789 results in "123***789".
            /// </summary>
            [LogMasked(ShowFirst = 3, ShowLast = 3)]
            public string? ShowFirstAndLastThreeAndDefaultMaskInTheMiddle { get; set; }

            /// <summary>
            ///  123456789 results in "123REMOVED789".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowFirst = 3, ShowLast = 3)]
            public string? ShowFirstAndLastThreeAndCustomMaskInTheMiddle { get; set; }

            /// <summary>
            ///  NOTE PreserveLength=true is ignored in this case
            ///  123456789 results in "123REMOVED789".
            /// </summary>
            [LogMasked(Text = "REMOVED", ShowFirst = 3, ShowLast = 3, PreserveLength = true)]
            public string? ShowFirstAndLastThreeAndCustomMaskInTheMiddle2 { get; set; }
        }
    }
}