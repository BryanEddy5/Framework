using System;
using System.Collections.Generic;
using Bogus;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    public class PiiEntityDataGenerator
    {
        public static PiiRoleContract GetRole(Faker faker)
        {
            // 50% chance to add another role
            if (faker.Random.Number(3) > 1)
            {
                return new PiiRoleContract
                {
                    Email = faker.Internet.Email(),
                    PhoneNumber = faker.Phone.PhoneNumber("7#########"),
                    PostalCode = PiiEntityDataGenerator.Get5DigitZipCode(faker),
                    WorkAddress = GetAddress(faker)
                };
            }

            return null;
        }

        public static DateTime GetBirthDate(Faker faker)
        {
            return faker.Date.Past(
                40,
                DateTime.SpecifyKind(DateTime.Parse("1980-1-1 10:00 AM"), DateTimeKind.Utc));
        }

        public static string Get5DigitZipCode(Faker faker)
        {
            return faker.Address.ZipCode("#####");
        }

        public static IList<string> GetPhoneNumbers(Faker faker)
        {
            return faker.Make(3, () => faker.Phone.PhoneNumber("7#########"));
        }

        public static IList<PiiPhoneContract> GetPhones(Faker faker, IList<string> phoneNumbers = null)
        {
            if (phoneNumbers == null || phoneNumbers.Count == 0)
            {
                phoneNumbers = GetPhoneNumbers(faker);
            }

            var labels = new string[] { "home", "mobile", "work" };
            var phones = new List<PiiPhoneContract>();

            for (var i = 0; i < phoneNumbers.Count; i++)
            {
                var p = new PiiPhoneContract
                {
                    PhoneNumber = phoneNumbers[i],
                    IsPrimary = faker.PickRandomParam(true, false),
                    IsMobile = faker.PickRandomParam(true, false),
                    Label = labels[i],
                    Extension = faker.PickRandomParam("222", "888", "999")
                };
                phones.Add(p);
            }

            if (!phones.Exists(x => x.IsPrimary == true))
            {
                phones[0].IsPrimary = true;
            }

            return phones;
        }

        public static IList<PiiEmailContract> GetEmails(Faker faker)
        {
            return faker.Make(3,
                (i) => new PiiEmailContract
                {
                    EmailAddress = faker.Internet.Email(),
                    IsPrimary = faker.PickRandomParam(true, false)
                });
        }

        public static PiiAddressContract GetAddress(Faker faker)
        {
            return new PiiAddressContract
            {
                AddressLine1 = faker.Address.StreetAddress(),
                AddressLine2 = faker.Address.SecondaryAddress(),
                City = faker.Address.City(),
                Country = faker.Address.Country(),
                County = faker.Address.County(),
                State = faker.Address.State()
            };
        }

        public static PiiIdsContract GetIDs(Faker faker)
        {
            return new PiiIdsContract()
            {
                MedicareId = faker.Random.String2(10),
                MemberId = faker.Random.String2(12)
            };
        }
    }
}