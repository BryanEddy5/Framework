using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// A generator of fake PiiEntities.
    /// </summary>
    public class PiiEntityDataGenerator
    {
        /// <summary>
        /// Generate a fake <see cref="PiiCoreContracts.PiiRoleContract"/>.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns>A fake <see cref="PiiCoreContracts.PiiRoleContract"/>.</returns>
        public static PiiCoreContracts.PiiRoleContract GetRole(Faker faker)
        {
            // 50% chance to add another role
            if (faker.Random.Number(3) > 1)
            {
                return new PiiCoreContracts.PiiRoleContract
                {
                    Email = faker.Internet.Email(),
                    PhoneNumber = faker.Phone.PhoneNumber("7#########"),
                    PostalCode = PiiEntityDataGenerator.Get5DigitZipCode(faker),
                    WorkAddress = GetAddress(faker)
                };
            }

            return null;
        }

        /// <summary>
        /// Returns a fake birth date in <see cref="DateTime"/> format.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns>A fake birth date in <see cref="DateTime"/> format.</returns>
        public static DateTime GetBirthDate(Faker faker)
        {
            return faker.Date.Past(
                40,
                DateTime.SpecifyKind(DateTime.Parse("1980-1-1 10:00 AM"), DateTimeKind.Utc));
        }

        /// <summary>
        /// Returns a fake zipcode in string format.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns>A fake zipcode in string format.</returns>
        public static string Get5DigitZipCode(Faker faker)
        {
            return faker.Address.ZipCode("#####");
        }

        /// <summary>
        /// Returns an <see cref="IList{T}"/> of fake phone numbers in string format.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns>An <see cref="IList{T}"/> of fake phone numbers in string format.</returns>
        public static IList<string> GetPhoneNumbers(Faker faker)
        {
            return faker.Make(3, () => faker.Phone.PhoneNumber("7#########"));
        }

        /// <summary>
        /// Return an <see cref="IList{T}"/> of fake <see cref="PiiCoreContracts.PiiPhoneContract"/>.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <param name="phoneNumbers">An <see cref="IList{T}"/> of phone numbers in string format.</param>
        /// <returns>An <see cref="IList{T}"/> of <see cref="PiiCoreContracts.PiiPhoneContract"/>.</returns>
        public static IList<PiiCoreContracts.PiiPhoneContract> GetPhones(Faker faker, IList<string> phoneNumbers = null)
        {
            if (phoneNumbers == null || phoneNumbers.Count == 0)
            {
                phoneNumbers = GetPhoneNumbers(faker);
            }

            var labels = new string[] { "home", "mobile", "work" };
            var phones = phoneNumbers.Select((t, i) =>
                new PiiCoreContracts.PiiPhoneContract
                {
                    PhoneNumber = t,
                    IsPrimary = faker.PickRandomParam(true, false),
                    IsMobile = faker.PickRandomParam(true, false),
                    Label = labels[i],
                    Extension = faker.PickRandomParam("222", "888", "999")
                }).ToList();

            if (!phones.Exists(x => x.IsPrimary == true))
            {
                phones[0].IsPrimary = true;
            }

            return phones;
        }

        /// <summary>
        /// Return an <see cref="IList{T}"/> of fake <see cref="PiiCoreContracts.PiiEmailContract"/>.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns>An <see cref="IList{T}"/> of <see cref="PiiCoreContracts.PiiEmailContract"/>.</returns>
        public static IList<PiiCoreContracts.PiiEmailContract> GetEmails(Faker faker)
        {
            return faker.Make(
                count: 3,
                (i) => new PiiCoreContracts.PiiEmailContract
                {
                    EmailAddress = faker.Internet.Email(),
                    IsPrimary = faker.PickRandomParam(true, false)
                });
        }

        /// <summary>
        /// Return a fake <see cref="PiiCoreContracts.PiiAddressContract"/>.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns><see cref="PiiCoreContracts.PiiAddressContract"/>.</returns>
        public static PiiCoreContracts.PiiAddressContract GetAddress(Faker faker)
        {
            return new PiiCoreContracts.PiiAddressContract
            {
                AddressLine1 = faker.Address.StreetAddress(),
                AddressLine2 = faker.Address.SecondaryAddress(),
                City = faker.Address.City(),
                Country = faker.Address.Country(),
                County = faker.Address.County(),
                State = faker.Address.State()
            };
        }

        /// <summary>
        /// Generate a fake <see cref="PiiCoreContracts.PiiIdsContract"/>.
        /// </summary>
        /// <param name="faker"><see cref="Faker"/>.</param>
        /// <returns><see cref="PiiCoreContracts.PiiIdsContract"/>.</returns>
        public static PiiCoreContracts.PiiIdsContract GetIDs(Faker faker)
        {
            return new PiiCoreContracts.PiiIdsContract()
            {
                MedicareId = faker.Random.String2(10),
                MemberId = faker.Random.String2(12)
            };
        }
    }
}