using HumanaEdge.Webcore.Core.Rest;
using HumanaEdge.Webcore.Core.Rest.AccessTokens;
using Microsoft.AspNetCore.Http;
using Polly;

namespace HumanaEdge.Webcore.Framework.Rest.Transformations
{
    /// <summary>
    /// A factory for generating <see cref="Context"/>.
    /// </summary>
    public interface IRequestTransformationFactory
    {
        /// <summary>
        /// Creates a <see cref="RequestTransformationService"/>.
        /// </summary>
        /// <param name="options">REST client options.</param>
        /// <returns><see cref="RequestTransformationService"/>.</returns>
        IRequestTransformationService Create(RestClientOptions options);
    }
}