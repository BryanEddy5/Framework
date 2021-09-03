using HumanaEdge.Webcore.Core.SecretsManager.Contracts;

namespace HumanaEdge.Webcore.Framework.SecretsManager.Tests.Stubs
{
    /// <summary>
    /// Shhhh it's a secret.
    /// </summary>
    public class FakeSecret : ISecret
    {
        /// <summary>
        /// Don't tell anyone this name.
        /// </summary>
        public string Name { get; set; }
    }
}