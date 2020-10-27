using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Autofac;
using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Framework.DependencyInjection
{
    /// <summary>
    /// Registers services in the DI container.
    /// </summary>
    public static class ComponentRegistration
    {
        /// <summary>
        /// Extension method for Autofac's ContainerBuilder called From Startup.ConfigureContainer.
        /// </summary>
        /// <typeparam name="TEntry">A type located in the entry assembly.</typeparam>
        /// <param name="builder">Autofac's ContainerBuilder.</param>
        internal static void RegisterComponents<TEntry>(this ContainerBuilder builder)
        {
            var assemblies =
                typeof(TEntry).Assembly.GetAssemblyAndDependencies(assembly => assembly.Name!.StartsWith("HumanaEdge"));
            var serviceRegistrationInfo = assemblies.SelectMany(GetRegistration).ToArray();
            foreach (var service in serviceRegistrationInfo)
            {
                builder.RegisterService(service);
            }
        }

        private static IReadOnlyList<ServiceRegistrationInfo> GetRegistration(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => t.GetCustomAttributes(false).OfType<IDiComponent>().Any())
                .SelectMany(GetRegistration)
                .ToArray();
        }

        private static IReadOnlyList<ServiceRegistrationInfo> GetRegistration(Type type)
        {
            return type.GetCustomAttributes(false)
                .OfType<IDiComponent>()
                .Select(att => GetRegistration(type, att))
                .ToArray();
        }

        /// <summary>
        /// Extracts the interface for the that the component should be injected for.
        /// </summary>
        private static ServiceRegistrationInfo GetRegistration(
            Type type,
            IDiComponent attribute)
        {
            var serviceType = attribute.Target;
            if (serviceType == null)
            {
                var directInterfaces = type.GetInterfaces()
                    .Except(type.BaseType?.GetInterfaces() ?? Array.Empty<Type>())
                    .ToArray();
                if (directInterfaces.Length > 1)
                {
                    throw new InvalidOperationException(
                        $"Multiple potential DI component targets detected for type {type.FullName}. Specify a Target explicitly");
                }

                if (directInterfaces.Length == 0)
                {
                    serviceType = type;
                }
                else
                {
                    serviceType = directInterfaces[0];
                }
            }

            return new ServiceRegistrationInfo(
                attribute.LifetimeScope,
                serviceType,
                type,
                attribute.AutowireProperties);
        }

        /// <summary>
        /// Register services within the DI conatiner.
        /// </summary>
        private static void RegisterService(
            this ContainerBuilder builder,
            ServiceRegistrationInfo registration)
        {
            var registrationBuilder = builder.RegisterType(registration.Implementation)
                .As(registration.Target);

            if (registration.AutoWireProperties)
            {
                registrationBuilder = registrationBuilder.PropertiesAutowired();
            }

            switch (registration.LifetimeScopeEnum)
            {
                case LifetimeScopeEnum.Transient:

                    registrationBuilder.InstancePerDependency();
                    break;

                case LifetimeScopeEnum.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;

                case LifetimeScopeEnum.Singleton:
                    registrationBuilder.SingleInstance();
                    break;

                default:
                    throw new InvalidEnumArgumentException(
                        $"{registration.LifetimeScopeEnum} is currently not a supported service lifetime.");
            }
        }
    }
}