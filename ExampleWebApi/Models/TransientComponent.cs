using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.ExampleWebApi.Models
{
    /// <summary>
    /// Test component for dependency injection with transient lifetime.
    /// </summary>
    [DependencyInjectedComponent]
    public class TransientComponent : ITransientService
    {
    }
}