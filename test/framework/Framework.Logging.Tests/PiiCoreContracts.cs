using System;
using Destructurama.Attributed;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// Core contract types used by Entity.
    /// </summary>
    public class PiiCoreContracts
    {
        /// <summary>
        /// The unique identifiers associated with the entity.
        /// </summary>
        public class PiiIdsContract
        {
            /// <summary>
            /// Social security number.
            /// </summary>
            [LogMasked(PreserveLength = true, ShowFirst = 3)]
            public virtual string SSN { get; set; }

            /// <summary>
            /// Medicare Id.
            /// </summary>
            [LogMasked(PreserveLength = true, ShowFirst = 3)]
            public string MedicareId { get; set; }

            /// <summary>
            /// Member Id.
            /// </summary>
            [LogMasked(PreserveLength = true, ShowFirst = 3)]
            public string MemberId { get; set; }
        }

        /// <summary>
        /// The various roles that an entity may have.
        /// </summary>
        public class PiiRoleContract
        {
            /// <summary>
            /// The role's email address.
            /// </summary>
            [LogMasked(Text = "foo@gmail.com")]
            public virtual string Email { get; set; }

            /// <summary>
            /// The role's phone number.
            /// </summary>
            [LogMasked(PreserveLength = true, ShowLast = 4)]
            public virtual string PhoneNumber { get; set; }

            /// <summary>
            /// The role's postal code.
            /// </summary>
            [LogMasked(PreserveLength = true, ShowFirst = 1)]
            public virtual string PostalCode { get; set; }

            /// <summary>
            /// The role's work address.
            /// </summary>
            public PiiAddressContract WorkAddress { get; set; }
        }

        /// <summary>
        /// Contract to describe a phone number.
        /// </summary>
        public class PiiPhoneContract
        {
            /// <summary>
            /// The phone number itself.
            /// </summary>
            [LogMasked(PreserveLength = true, ShowLast = 4)]
            public virtual string PhoneNumber { get; set; }

            /// <summary>
            /// The extension.
            /// </summary>
            public virtual string Extension { get; set; }

            /// <summary>
            /// String label to distinguish between work, home, cell, etc.
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// True if this phone number is for a mobile phone.
            /// </summary>
            public bool IsMobile { get; set; }

            /// <summary>
            /// True if this phone number is the primary phone number in a collection of phone numbers.
            /// </summary>
            public bool IsPrimary { get; set; }

            /// <summary>
            /// The date that this phone number was last updated.
            /// </summary>
            public DateTimeOffset UpdatedAt { get; set; }
        }

        /// <summary>
        /// Contract to describe an email address.
        /// </summary>
        public class PiiEmailContract
        {
            /// <summary>
            /// The actual email address.
            /// </summary>
            [LogMasked(Text = "foo@gmail.com")]
            public virtual string EmailAddress { get; set; }

            /// <summary>
            /// True if this is the primary email address in a collection of email addresses.
            /// </summary>
            public bool IsPrimary { get; set; }
        }

        /// <summary>
        /// All of the possible roles for an entity.
        /// </summary>
        /// <typeparam name="TRole">A role type that implements <see cref="PiiRoleContract"/>.</typeparam>
        public class PiiRolesContract<TRole>
            where TRole : PiiRoleContract
        {
            /// <summary>
            /// An enrolled member.
            /// </summary>
            public TRole Member { get; set; }

            /// <summary>
            /// Care giver.
            /// </summary>
            public TRole Caregiver { get; set; }

            /// <summary>
            /// A provider.
            /// </summary>
            public TRole Provider { get; set; }

            /// <summary>
            /// The care coordinator role type.
            /// </summary>
            public TRole CareCoordinator { get; set; }

            /// <summary>
            /// A social worker.
            /// </summary>
            public TRole SocialWorker { get; set; }

            /// <summary>
            /// Community health worker.
            /// </summary>
            public TRole CommunityHealthWorker { get; set; }

            /// <summary>
            /// A pharmacist.
            /// </summary>
            public TRole Pharmacist { get; set; }

            /// <summary>
            /// Care team specialist.
            /// </summary>
            public TRole CareTeamSpecialist { get; set; }

            /// <summary>
            /// A nurse.
            /// </summary>
            public TRole Nurse { get; set; }

            /// <summary>
            /// A nurse preceptor.
            /// </summary>
            public TRole NursePreceptor { get; set; }
        }

        /// <summary>
        /// The contract that describes a physical address.
        /// </summary>
        public class PiiAddressContract
        {
            /// <summary>
            /// Address line 1.
            /// </summary>
            [LogMasked(Text = "AddressLine1")]
            public string AddressLine1 { get; set; }

            /// <summary>
            /// Address line 2.
            /// </summary>
            [LogMasked(Text = "AddressLine2")]
            public string AddressLine2 { get; set; }

            /// <summary>
            /// City.
            /// </summary>
            [LogMasked(Text = "City")]

            public string City { get; set; }

            /// <summary>
            /// State.
            /// </summary>
            public string State { get; set; }

            /// <summary>
            /// County.
            /// </summary>
            public string County { get; set; }

            /// <summary>
            /// Country.
            /// </summary>
            public string Country { get; set; }
        }
    }
}