namespace HumanaEdge.Webcore.Core.Testing.Transformations
{
    /// <summary>
    /// Associates name test clients with a service.
    /// </summary>
    public interface INamedClientService
    {
        /// <summary>
        /// The client names that the service will used for.
        /// </summary>
        string[] ClientNames { get; }
    }
}