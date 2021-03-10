using System.IO;
using HumanaEdge.Webcore.Core.Testing.Integration;
using HumanaEdge.Webcore.Framework.Testing.EnvironmentSetup;
using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Framework.Testing.Integration.Data
{
    /// <inheritdoc />
    internal sealed class TestData<TData> : ITestData<TData>
        where TData : class
    {
        private static TData? _value;

        private readonly object _objectLock = new object();

        /// <inheritdoc/>
        public TData Get
        {
            get
            {
                if (_value == null)
                {
                    lock (_objectLock)
                    {
                        var env = TestEnvironmentHandler.GetEnvironment;
                        _value ??= JsonConvert.DeserializeObject<TData>(File.ReadAllText($"Configuration.{env}.json"));
                    }
                }

                return _value;
            }
        }
    }
}