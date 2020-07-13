using System;

namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <summary>
    /// [DependencyInjected] attribute identifies a class that can be automatically injected
    /// Classes with this attribute are detected via assembly scanning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DependencyInjectedComponentAttribute : System.Attribute
    {
        /// <summary>
        /// The ctor.
        /// </summary>
        /// <param name="lifetimeScope">The desired lifetime for the component to be registered.</param>
        public DependencyInjectedComponentAttribute(LifetimeScopeEnum lifetimeScope = LifetimeScopeEnum.Transient)
        {
            LifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// returns the desired LifetimeScope of the component to be registered.
        /// </summary>
        public LifetimeScopeEnum LifetimeScope { get; }
    }
}