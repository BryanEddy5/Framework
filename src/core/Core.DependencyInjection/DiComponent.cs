using System;

namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DiComponent : Attribute, IDiComponent
    {
        /// <inheritdoc />
        public bool AutowireProperties { get; set; }

        /// <inheritdoc />
        public LifetimeScopeEnum LifetimeScope { get; set; }

        /// <inheritdoc />
        public Type? Target { get; set; }
    }
}