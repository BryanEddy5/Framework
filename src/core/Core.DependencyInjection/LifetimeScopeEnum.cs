namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <summary>
    /// Enum to indicate component lifetime when registering it as a DI'd component.
    /// Here is the mapping of Microsoft's component lifetime enums to Autofac's:
    ///     Transient == InstancePerDependency
    ///     Scoped == InstancePerLifetimeScope
    ///     Singleton == SingleInstance
    /// .
    /// </summary>
    public enum LifetimeScopeEnum
    {
        /// <summary>
        /// Specifies instance per resolution
        /// </summary>
        Transient,

        /// <summary>
        /// Specifies instance per request (such as in a WebApi call)
        /// </summary>
        Scoped,

        /// <summary>
        /// Specifies a singleton
        /// </summary>
        Singleton
    }
}