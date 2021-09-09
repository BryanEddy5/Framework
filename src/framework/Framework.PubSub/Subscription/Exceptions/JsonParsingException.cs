using System;
using HumanaEdge.Webcore.Core.PubSub;
using HumanaEdge.Webcore.Core.PubSub.Exceptions;

namespace HumanaEdge.Webcore.Framework.PubSub.Subscription.Exceptions
{
    /// <summary>
    /// A custom exception thrown when there is an issue parsing json.
    /// </summary>
    public class JsonParsingException : PubSubException
    {
        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="exception">The inner exception.</param>
        public JsonParsingException(Exception exception)
            : base("There was an error parsing the json", exception)
        {
        }

        /// <inheritdoc />
        public override Reply Reply => Reply.Ack;
    }
}