using System;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// EntityView response model that API returns.
    /// </summary>
    public class PiiEntityResponseContract : PiiBaseEntityContract<PiiCoreContracts.PiiIdsContract, PiiCoreContracts.PiiRoleContract, PiiCoreContracts.PiiPhoneContract, PiiCoreContracts.PiiEmailContract>
    {
        /// <summary>
        /// Date the entity was created in <see cref="DateTimeOffset"/> format.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Date the entity was updated in <see cref="DateTimeOffset"/> format.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }
}