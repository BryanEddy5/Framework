namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <summary>
    ///     An attribute used to configuring the Options Pattern with the dependency injection container during startup.
    /// </summary>
    public interface IDiOptionsAttribute
    {
        /// <summary>
        ///     The configuration key for retrieving the configuration section.
        /// </summary>
        string Key { get; }
    }
}