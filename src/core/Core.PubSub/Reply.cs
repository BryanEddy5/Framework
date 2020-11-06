namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// Reply from a message handler; whether to <see cref="Ack"/>
    /// or <see cref="Nack"/> the message to the server.
    /// </summary>
    public enum Reply
    {
        /// <summary>
        /// Message not handled successfully.
        /// </summary>
        Nack = 0,

        /// <summary>
        /// Message handled successfully.
        /// </summary>
        Ack = 1,
    }
}