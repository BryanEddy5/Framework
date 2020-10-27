using System;

namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <summary>
    /// Abstract interface for attributes which are used to register components with the DI container.
    /// </summary>
    public interface IDiComponent
    {
        /// <summary>
        /// Indicates if properties of the service should be resolved through the service container during construction.
        /// </summary>
        bool AutowireProperties { get; }

        /// <summary>
        /// The lifetime of the service implementation in the DI container.
        /// </summary>
        LifetimeScopeEnum LifetimeScope { get; }

        /// <summary>
        /// The service type this implementation is being registered against. If null, the service is assumed to directly
        /// implement a single interface and it will be registered against that interface.
        /// </summary>
        Type? Target { get; }
    }
}