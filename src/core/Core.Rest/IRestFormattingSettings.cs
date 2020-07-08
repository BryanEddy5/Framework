using Newtonsoft.Json;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    ///     Settings information for <see cref="IMediaTypeFormatter" /> instances.
    /// </summary>
    public interface IRestFormattingSettings
    {
        /// <summary>
        ///     Settings for JSON formatting.
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}