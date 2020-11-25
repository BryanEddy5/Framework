using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Example.WebApi.DependencyInjection
{
    /// <summary>
    /// Test component for dependency injection with scoped lifetime.
    /// </summary>
    [DiComponent(LifetimeScope = LifetimeScopeEnum.Scoped)]
    public class ScopedComponent : IScopedService
    {
    }
}