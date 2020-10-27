using Autofac;

namespace HumanaEdge.Webcore.Framework.DependencyInjection.Extensions
{
    /// <summary>
    /// Builder of container for component registration.
    /// </summary>
    internal static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Extension method for Autofac's ContainerBuilder called From Startup.ConfigureContainer.
        /// </summary>
        /// <typeparam name="TEntry">A type located in the entry assembly.</typeparam>
        /// <param name="builder">Autofac's ContainerBuilder.</param>
        internal static void RegisterWebcoreAttributedComponents<TEntry>(this ContainerBuilder builder)
        {
            builder.RegisterComponents<TEntry>();
        }
    }
}