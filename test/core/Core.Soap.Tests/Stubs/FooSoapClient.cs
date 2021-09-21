using HumanaEdge.Webcore.Core.Soap.Client;
using HumanaEdge.Webcore.Core.Soap.Client.Factory;

namespace HumanaEdge.Webcore.Core.Soap.Tests.Stubs
{
    /// <summary>
    /// A stub soap client for testing <see cref="BaseSoapClient{TClient,TChannel}"/>.
    /// </summary>
    public sealed class FooSoapClient
        : BaseSoapClient<FooSoapClient, IBarSoapReference>, IFooSoapClient
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mockBehaviorFactory">The endpoint behavior factory.</param>
        /// <param name="fakeOptions">The options.</param>
        public FooSoapClient(
            IEndpointBehaviorFactory mockBehaviorFactory,
            SoapClientOptions fakeOptions)
            : base(mockBehaviorFactory, fakeOptions)
        {
        }

        /// <summary>
        /// Returns the string of "hello world".
        /// </summary>
        /// <returns>The "hello world" string.</returns>
        public string HelloWorld()
        {
            return Channel.HelloWorld();
        }
    }
}