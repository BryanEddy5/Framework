namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests.Stubs
{
    /// <summary>
    /// A generic interface for a dependency.
    /// </summary>
    public interface IDependency
    {
        /// <summary>
        /// An action of <see cref="IDependency" />.
        /// </summary>
        void DoWork();
    }
}