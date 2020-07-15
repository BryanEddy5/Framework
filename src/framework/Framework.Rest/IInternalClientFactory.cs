using System;
using System.Net.Http;

namespace HumanaEdge.Webcore.Framework.Rest
{
    /// <summary>
    /// Factory pattern for generating an <see cref="IInternalClient"/>.
    /// </summary>
    internal interface IInternalClientFactory
    {
        /// <summary>
        /// Creates a <see cref="IInternalClient"/> based off of the inputs.
        /// </summary>
        /// <param name="clientName">The named type instance of an <see cref="HttpClient"/>.</param>
        /// <param name="baseUri">The base URI of the request.</param>
        /// <param name="timeout">The time duration until a timeout occurs for the request.</param>
        /// <returns>A <see cref="IInternalClient"/> for sending RESTful http requests.</returns>
        IInternalClient CreateClient(string clientName, Uri baseUri, TimeSpan timeout);
    }
}