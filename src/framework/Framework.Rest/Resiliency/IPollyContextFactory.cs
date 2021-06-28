using Polly;

namespace HumanaEdge.Webcore.Framework.Rest.Resiliency
{
    /// <summary>
    /// A factory for generating <see cref="Context"/>.
    /// </summary>
    public interface IPollyContextFactory
    {
        /// <summary>
        /// Creates the <see cref="Context"/>.
        /// </summary>
        /// <returns>The Polly <see cref="Context"/>.</returns>
        public Context Create();
    }
}