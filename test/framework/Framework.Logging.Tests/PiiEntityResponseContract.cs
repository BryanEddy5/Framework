using System;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// EntityView response model that API returns.
    /// </summary>
    public class PiiEntityResponseContract : PiiBaseEntityContract<PiiIdsContract, PiiRoleContract, PiiPhoneContract, PiiEmailContract>
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}