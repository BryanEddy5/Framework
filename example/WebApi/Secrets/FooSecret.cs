using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Example.WebApi.Secrets
{
    /// <summary>
    /// Some fake secret shape Foo.
    /// </summary>
    public class FooSecret : ISecret
    {
        /// <summary>
        /// Some top secret being pulled from Secrets Manager.
        /// </summary>
        public string SuperSecret { get; set; }
    }
}