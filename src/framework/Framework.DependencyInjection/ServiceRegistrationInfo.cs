using System;
using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.DependencyInjection
{
    /// <summary>
    /// Service registration information used for DI.
    /// </summary>
    public class ServiceRegistrationInfo
    {
        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="lifetimeScopeEnum">The lifetime scope of the service.</param>
        /// <param name="target">The interface that will have the actual service injected.</param>
        /// <param name="implementation">The service to be injected.</param>
        /// <param name="autoWireProperties">Allows for property injection if set.</param>
        public ServiceRegistrationInfo(
            LifetimeScopeEnum lifetimeScopeEnum,
            Type target,
            Type implementation,
            bool autoWireProperties)
        {
            LifetimeScopeEnum = lifetimeScopeEnum;
            Target = target;
            Implementation = implementation;
            AutoWireProperties = autoWireProperties;
        }

        /// <summary>
        /// Allows for property injection if set.
        /// </summary>
        public bool AutoWireProperties { get; }

        /// <summary>
        /// The lifetime scope of the service.
        /// </summary>
        public LifetimeScopeEnum LifetimeScopeEnum { get; }

        /// <summary>
        /// The interface that will have the actual service injected.
        /// </summary>
        public Type Target { get; }

        /// <summary>
        /// The service to be injected.
        /// </summary>
        public Type Implementation { get; }
    }
}