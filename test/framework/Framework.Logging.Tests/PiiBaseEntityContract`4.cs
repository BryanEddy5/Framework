using System.Collections.Generic;
using Destructurama.Attributed;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// PiiBaseEntityContract allows PII-masked entities vs as-stored entities.
    /// </summary>
    /// <typeparam name="TIds">Ids type (PII-masked or otherwise).</typeparam>
    /// <typeparam name="TRole">Role type (PII-masked or otherwise).</typeparam>
    /// <typeparam name="TPhone">Phone type (PII-masked or otherwise).</typeparam>
    /// <typeparam name="TEmail">Email type (PII-masked or otherwise).</typeparam>
    public class PiiBaseEntityContract<TIds, TRole, TPhone, TEmail>
        where TIds : PiiCoreContracts.PiiIdsContract
        where TRole : PiiCoreContracts.PiiRoleContract
        where TPhone : PiiCoreContracts.PiiPhoneContract
        where TEmail : PiiCoreContracts.PiiEmailContract
    {
        /// <summary>
        /// The unique identifier for an entity.
        /// </summary>
        public virtual string EntityId { get; set; }

        /// <summary>
        /// The identifier.
        /// </summary>
        public TIds Ids { get; set; }

        /// <summary>
        /// The first name of the entity.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name.
        /// </summary>
        [LogMasked]
        public string LastName { get; set; }

        /// <summary>
        /// Middle name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Preferred name.
        /// </summary>
        public string PreferredName { get; set; }

        /// <summary>
        /// The entity's work title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The name suffix.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// The gender of the entity.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// The individual's birthdate.
        /// </summary>
        [LogMasked]
        public virtual string BirthDate { get; set; }

        /// <summary>
        /// Zipcode.
        /// </summary>
        [LogMasked(PreserveLength = true, ShowFirst = 1)]
        public virtual string Zipcode { get; set; }

        /// <summary>
        /// Phone numbers.
        /// </summary>
        public virtual List<TPhone> Phones { get; set; }

        /// <summary>
        /// A collection of emails associated with the entity.
        /// </summary>
        public List<TEmail> Emails { get; set; }

        /// <summary>
        /// Home address.
        /// </summary>
        public PiiCoreContracts.PiiAddressContract HomeAddress { get; set; }

        /// <summary>
        /// The possible roles associated with the entity.
        /// </summary>
        public PiiCoreContracts.PiiRolesContract<TRole> Roles { get; set; }
    }
}