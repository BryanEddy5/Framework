using System.Net;
using HumanaEdge.Webcore.Core.Common.Exceptions;
using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Client.Models;

namespace HumanaEdge.Webcore.Core.Soap.Exceptions
{
    /// <summary>
    /// Thrown when the <see cref="SoapClientOptions.BaseEndpoint"/> uses an unsupported scheme,
    /// such as "ftp://" or "gs://".
    /// </summary>
    public class DuplicateSoapHeaderException : MessageAppException
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="soapHeader">The SOAP header that is a duplicate.</param>
        public DuplicateSoapHeaderException(SoapHeader soapHeader)
            : base($"The header value {soapHeader.Name} is already defined. Only one header value can be defined per header key.")
        {
        }

        /// <inheritdoc/>
        public override HttpStatusCode StatusCode => HttpStatusCode.NotImplemented;
    }
}