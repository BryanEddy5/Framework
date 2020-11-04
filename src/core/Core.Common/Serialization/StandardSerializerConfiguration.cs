using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace HumanaEdge.Webcore.Core.Common.Serialization
{
    /// <summary>
    /// Standardized serializer configuration settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class StandardSerializerConfiguration
    {
        /// <summary>
        /// Standardized <see cref="JsonSerializerSettings"/>.
        /// </summary>
        public static JsonSerializerSettings Settings => new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() }
        };
    }
}