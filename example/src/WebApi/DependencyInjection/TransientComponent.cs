using HumanaEdge.Webcore.Core.DependencyInjection;

namespace HumanaEdge.Webcore.Example.WebApi.DependencyInjection
{
    /// <summary>
    /// Test component for dependency injection with transient lifetime.
    /// </summary>
    [DiComponent]
    public class TransientComponent : ITransientService
    {
    }
}