namespace HumanaEdge.Webcore.Core.Encryption
{
    /// <summary>
    /// Options object utilized in the EncryptionService that holds required information on how to interact with KMS.
    /// </summary>
    public class EncryptionServiceOptions
    {
        /// <summary>
        /// GCP Project in which the KMS resource is provisioned.
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// GCP region in which the KMS resource is provisioned.
        /// </summary>
        public string? LocationId { get; set; }

        /// <summary>
        /// Id of the key ring in the associated KMS.
        /// </summary>
        public string? KeyRingId { get; set; }

        /// <summary>
        /// Id of the key in the associated KMS.
        /// </summary>
        public string? KeyId { get; set; }
    }
}