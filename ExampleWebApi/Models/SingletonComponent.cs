using HumanaEdge.Webcore.Core.DependencyInjection;

namespace ExampleWebApi.Models
{
    /// <summary>
    /// Test component for dependency injection with singleton lifetime.
    /// </summary>
    [DependencyInjectedComponent(LifetimeScopeEnum.Singleton)]
    public class SingletonComponent : ISingletonService
    {
    }
}