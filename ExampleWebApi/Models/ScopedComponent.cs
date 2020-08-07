using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.ExampleWebApi.Models
{
    /// <summary>
    /// Test component for dependency injection with scoped lifetime.
    /// </summary>
    [DependencyInjectedComponent(LifetimeScopeEnum.Scoped)]
    public class ScopedComponent : IScopedService
    {
    }
}