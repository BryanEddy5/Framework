using System;
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
        where TIds : PiiIdsContract
        where TRole : PiiRoleContract
        where TPhone : PiiPhoneContract
        where TEmail : PiiEmailContract
    {
        public virtual string EntityId { get; set; }
        public TIds Ids { get; set; }
        public string FirstName { get; set; }
        [LogMasked]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PreferredName { get; set; }
        public string Title { get; set; }
        public string Suffix { get; set; }
        public string Gender { get; set; }
        [LogMasked]
        public virtual string BirthDate { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 1)]
        public virtual string Zipcode { get; set; }
        public virtual List<TPhone> Phones { get; set; }
        public List<TEmail> Emails { get; set; }
        public PiiAddressContract HomeAddress { get; set; }
        public PiiRolesContract<TRole> Roles { get; set; }
    }

    public class PiiIdsContract
    {
        [LogMasked(PreserveLength = true, ShowFirst = 3)]
        public virtual string SSN { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 3)]
        public string MedicareId { get; set; }

        [LogMasked(PreserveLength = true, ShowFirst = 3)]
        public string MemberId { get; set; }
    }

    /// <summary>
    /// The various roles that an entity may have.
    /// </summary>
    public class PiiRoleContract
    {
        [LogMasked(Text = "foo@gmail.com")]
        public virtual string Email { get; set; }
        [LogMasked(PreserveLength = true, ShowLast = 4)]
        public virtual string PhoneNumber { get; set; }
        [LogMasked(PreserveLength = true, ShowFirst = 1)]
        public virtual string PostalCode { get; set; }
        public PiiAddressContract WorkAddress { get; set; }
    }

    public class PiiPhoneContract
    {
        [LogMasked(PreserveLength = true, ShowLast = 4)]
        public virtual string PhoneNumber { get; set; }
        public virtual string Extension { get; set; }
        public string Label { get; set; }
        public bool IsMobile { get; set; }
        public bool IsPrimary { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class PiiEmailContract
    {
        [LogMasked(Text="foo@gmail.com")]
        public virtual string EmailAddress { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class PiiRolesContract<TRole> where TRole : PiiRoleContract
    {
        public TRole Member { get; set; }
        public TRole Caregiver { get; set; }
        public TRole Provider { get; set; }
        public TRole CareCoordinator { get; set; }
        public TRole SocialWorker { get; set; }
        public TRole CommunityHealthWorker { get; set; }
        public TRole Pharmacist { get; set; }
        public TRole CareTeamSpecialist { get; set; }
        public TRole Nurse { get; set; }
        public TRole NursePreceptor { get; set; }
    }

    public class PiiAddressContract
    {
        [LogMasked(Text = "AddressLine1")]
        public string AddressLine1 { get; set; }
        [LogMasked(Text = "AddressLine2")]

        public string AddressLine2 { get; set; }
        [LogMasked(Text = "City")]

        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
    }
}