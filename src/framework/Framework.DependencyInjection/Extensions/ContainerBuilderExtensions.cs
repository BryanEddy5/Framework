using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Autofac;
using HumanaEdge.Webcore.Core.DependencyInjection;

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
            var assemblies = typeof(TEntry).Assembly.GetAssemblyAndDependencies(assembly => assembly.Name!.StartsWith("HumanaEdge"));
            var typesWithTransientLifetimes = new List<Type>();
            var typesWithScopedLifetimes = new List<Type>();
            var typesWithSingletonLifetimes = new List<Type>();

            foreach (var assembly in assemblies)
            {
                foreach (var t in assembly.GetTypes())
                {
                    var attribute = t.GetCustomAttribute<DependencyInjectedComponentAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    switch (attribute.LifetimeScope)
                    {
                        case LifetimeScopeEnum.Transient:
                            typesWithTransientLifetimes.Add(t);
                            break;

                        case LifetimeScopeEnum.Scoped:
                            typesWithScopedLifetimes.Add(t);
                            break;

                        case LifetimeScopeEnum.Singleton:
                            typesWithSingletonLifetimes.Add(t);
                            break;

                        default:
                            throw new InvalidEnumArgumentException(
                                $"{attribute.LifetimeScope} is currently not a supported service lifetime.");
                    }
                }
            }

            // Here is the mapping of Microsoft's component lifetime enums to Autofac's
            // Transient == InstancePerDependency
            // Scoped == InstancePerLifetimeScope
            // Singleton == SingleInstance
            builder.RegisterTypes(typesWithTransientLifetimes.ToArray())
                .AsImplementedInterfaces()
                .InstancePerDependency();
            builder.RegisterTypes(typesWithScopedLifetimes.ToArray())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterTypes(typesWithSingletonLifetimes.ToArray()).AsImplementedInterfaces().SingleInstance();
        }
    }
}