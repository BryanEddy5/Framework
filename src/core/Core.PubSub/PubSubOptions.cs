namespace HumanaEdge.Webcore.Core.PubSub
{
    /// <summary>
    /// A config object that holds the required info to connect to a PubSub resource.
    /// </summary>
    public class PubSubOptions
    {
        /// <summary>
        /// The Id of the GCP Project in which the resource lives.
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// The name of the resource in GCP.
        /// </summary>
        public string? Name { get; set; }
    }
}