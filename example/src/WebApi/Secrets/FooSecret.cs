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
        public string? SuperSecret { get; set; }

        /// <summary>
        /// Another fantastic, super, amazing secret to be retrieved.
        /// </summary>
        public string? SomeOtherAmazingSecret { get; set; }

        /// <summary>
        /// A nested secret.
        /// </summary>
        public NestedSecretClass? NestedSecret { get; set; }

        /// <summary>
        /// Holds a nested secret.
        /// </summary>
        public class NestedSecretClass
        {
            /// <summary>
            /// Some nested secret value.
            /// </summary>
            public int? NumberOfAmazingSecrets { get; set; }
        }
    }
}