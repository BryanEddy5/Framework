using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Example.WebApi.DependencyInjection
{
    /// <summary>
    /// Test component for dependency injection with singleton lifetime.
    /// </summary>
    [DiComponent(LifetimeScope = LifetimeScopeEnum.Singleton)]
    public class SingletonComponent : ISingletonService
    {
    }
}