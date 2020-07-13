using HumanaEdge.Webcore.Core.DependencyInjection;

namespace ExampleWebApi.Models
{
    /// <summary>
    /// Test component for dependency injection with transient lifetime.
    /// </summary>
    [DependencyInjectedComponent(LifetimeScopeEnum.Transient)]
    public class TransientComponent : ITransientService
    {
    }
}