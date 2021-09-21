using System.Net;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Soap.Client;

namespace HumanaEdge.Webcore.Core.Soap.Exceptions
{
    /// <summary>
    /// Thrown when the <see cref="SoapClientOptions.BaseEndpoint"/> uses an unsupported scheme,
    /// such as "ftp://" or "gs://".
    /// </summary>
    public class UnsupportedSchemeException : MessageAppException
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="unsupportedScheme">The unsupported scheme that resulted in this exception.</param>
        public UnsupportedSchemeException(string unsupportedScheme)
            : base($"The scheme of {unsupportedScheme} is not supported by our SOAP client.")
        {
        }

        /// <inheritdoc/>
        public override HttpStatusCode StatusCode => HttpStatusCode.NotImplemented;
    }
}