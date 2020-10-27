using System;

namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DependencyInjectedComponentAttribute : DiComponent
    {
        /// <summary>
        /// The ctor.
        /// </summary>
        /// <param name="lifetimeScope">The desired lifetime for the component to be registered.</param>
        public DependencyInjectedComponentAttribute(LifetimeScopeEnum lifetimeScope = LifetimeScopeEnum.Transient)
        {
            LifetimeScope = lifetimeScope;
        }
    }
}