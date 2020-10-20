using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Tests.Stubs
{
    /// <summary>
    /// A class decorated with <see cref="DiOptionsAttribute" /> for testing.
    /// </summary>
    [DiOptions]
    public class FooClientOptions
    {
        /// <summary>
        /// Some property for testing.
        /// </summary>
        public string Name { get; set; }
    }
}