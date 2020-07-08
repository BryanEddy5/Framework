using System;

namespace HumanaEdge.Webcore.Core.Rest
{
    /// <summary>
    ///     Deserializes RESTful responses.
    /// </summary>
    public interface IRestResponseDeserializer
    {
        /// <summary>
        ///     The bytes from the http response message.
        /// </summary>
        byte[] ResponseBytes { get; }

        /// <summary>
        ///     Converts the RESTful response to a designated <see cref="Type" />.
        /// </summary>
        /// <typeparam name="TResponse">The <see cref="Type" /> to be deserialized to.</typeparam>
        /// <returns>The designated <see cref="Type" /> to be deserialized.</returns>
        TResponse ConvertTo<TResponse>();
    }
}