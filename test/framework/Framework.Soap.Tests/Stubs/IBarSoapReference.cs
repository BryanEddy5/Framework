using System.ServiceModel;

namespace HumanaEdge.Webcore.Framework.Soap.Tests.Stubs
{
    /// <summary>
    /// A stub interface that contains reference to a single method.
    /// </summary>
    [ServiceContract(ConfigurationName="BarSoapReference")]
    public interface IBarSoapReference
    {
        /// <summary>
        /// A hello world.
        /// </summary>
        /// <returns>"hello world".</returns>
        [OperationContract(Action="http://foo.bar/hello", ReplyAction="*")]
        string HelloWorld();
    }
}