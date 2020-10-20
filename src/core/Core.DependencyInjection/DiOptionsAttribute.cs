using System;

namespace HumanaEdge.Webcore.Core.DependencyInjection
{
    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class DiOptionsAttribute : Attribute, IDiOptionsAttribute
    {
        /// <inheritdoc />
        public string Key { get; set; } = null!;
    }
}