using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HumanaEdge.Webcore.Core.Common.Extensions;
using HumanaEdge.Webcore.Core.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.DependencyInjection
{
    /// <summary>
    /// A service for registering configuration options in the DI container.
    /// </summary>
    internal static class OptionServicesRegistration
    {
        /// <summary>
        /// The <see cref="MethodInfo" /> of
        /// <see cref="OptionsConfigurationServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, IConfiguration)" />
        /// that will be invoked for DI registration.
        /// </summary>
        private static MethodInfo? ConfigureMethodInfo =>
            typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod(
                nameof(
                    OptionsConfigurationServiceCollectionExtensions.Configure),
                new[] { typeof(IServiceCollection), typeof(IConfiguration) });

        /// <summary>
        /// Adds the options classes to the DI container.
        /// </summary>
        /// <param name="services">The running service collection.</param>
        /// <param name="configuration">The configuration file of the application.</param>
        /// <param name="assemblies">The list of assemblies to scan.</param>
        internal static void AddOptionServices(
            this IServiceCollection services,
            IConfiguration configuration,
            Assembly[] assemblies)
        {
            services.AddOptions();
            var optionTypes = assemblies.SelectMany(GetOptionDecoratedTypes);
            optionTypes.ForEach(x => ConfigureOptions(x, services, configuration));
        }

        /// <summary>
        /// Invoke the
        /// <see cref="OptionsConfigurationServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, IConfiguration)" />
        /// method to
        /// register the Options service.
        /// </summary>
        /// <param name="optionType">The class <see cref="Type" /> of the classes to be registered.</param>
        /// <param name="services">The running service collection.</param>
        /// <param name="configuration">The configuration file of the application.</param>
        private static void ConfigureOptions(Type optionType, IServiceCollection services, IConfiguration configuration)
        {
            var myAttribute = optionType.GetCustomAttributes(false).OfType<IDiOptionsAttribute>().FirstOrDefault();
            var settingsSectionName = myAttribute?.Key ?? optionType.Name;

            var genericConfigureMethodInfo = ConfigureMethodInfo?.MakeGenericMethod(optionType);
            genericConfigureMethodInfo?.Invoke(
                null,
                new object[] { services, configuration.GetSection(settingsSectionName) });
        }

        /// <summary>
        /// Scan assembly for classes with the <see cref="DiOptionsAttribute" /> decorator.
        /// </summary>
        /// <param name="assembly">The assembly to be scanned. </param>
        /// <returns>A collection of <see cref="Type" />s that have the <see cref="DiOptionsAttribute" />.</returns>
        private static IEnumerable<Type> GetOptionDecoratedTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(false).OfType<IDiOptionsAttribute>().Any());
        }
    }
}