using System;
using HumanaEdge.Webcore.Core.Rest;

namespace HumanaEdge.Webcore.Core.Testing
{
    /// <summary>
    /// Creates a fake <see cref="IRestResponseDeserializer" /> for testing.
    /// </summary>
    public class TestRestResponseDeserializer : IRestResponseDeserializer
    {
        /// <summary>
        /// The delegate for generating the response object.
        /// </summary>
        private readonly Func<Type, object> _responseGenerator;

        /// <summary>
        /// Designated ctor.
        /// </summary>
        /// <param name="responseGenerator">Generates the response object.</param>
        /// <param name="fakeBytes">The fake bytes data for testing.</param>
        public TestRestResponseDeserializer(Func<Type, object> responseGenerator, byte[] fakeBytes)
        {
            _responseGenerator = responseGenerator;
            ResponseBytes = fakeBytes;
        }

        /// <inheritdoc/>
        public byte[] ResponseBytes { get; }

        /// <inheritdoc/>
        public TResponse ConvertTo<TResponse>()
        {
            return (TResponse)_responseGenerator(typeof(TResponse));
        }
    }
}