using System.Linq;
using AutoBogus;
using Bogus;
using Bogus.DataSets;
using Swashbuckle.AspNetCore.Filters;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    public class PiiEntityResponseContractExample : IExamplesProvider<PiiEntityResponseContract>
    {
        private static object _lock = new object();
        private static readonly Faker<PiiEntityResponseContract> _entityFaker;

        static PiiEntityResponseContractExample()
        {
            AutoFaker.Configure(builder => { builder.WithSkip<PiiEntityResponseContract>("EntityId"); });

            _entityFaker = new AutoFaker<PiiEntityResponseContract>()
                .RuleFor(e => e.BirthDate, f => PiiEntityDataGenerator.GetBirthDate(f).ToString("yyyy-MM-dd"))
                .RuleFor(e => e.Zipcode, PiiEntityDataGenerator.Get5DigitZipCode)
                .RuleFor(e => e.Gender, f => f.PickRandom<Name.Gender>().ToString())
                .RuleFor(
                    e => e.Phones,
                    f => PiiEntityDataGenerator.GetPhones(f).Select(phone => phone).ToList())
                .RuleFor(e => e.Roles, f => new PiiRolesContract<PiiRoleContract>
                {
                    Caregiver = PiiEntityDataGenerator.GetRole(f),
                    Member = PiiEntityDataGenerator.GetRole(f),
                    CareCoordinator = PiiEntityDataGenerator.GetRole(f),
                    Provider = PiiEntityDataGenerator.GetRole(f),
                    SocialWorker = PiiEntityDataGenerator.GetRole(f),
                    CommunityHealthWorker = PiiEntityDataGenerator.GetRole(f),
                    Pharmacist = PiiEntityDataGenerator.GetRole(f),
                    CareTeamSpecialist = PiiEntityDataGenerator.GetRole(f),
                    Nurse = PiiEntityDataGenerator.GetRole(f),
                })
                .RuleFor(
                    e => e.Emails,
                    f => PiiEntityDataGenerator.GetEmails(f).Select(email => email).ToList())
                .RuleFor(e => e.Ids, GetIDs)
                .RuleFor(e => e.HomeAddress, PiiEntityDataGenerator.GetAddress);
        }

        public PiiEntityResponseContract GetExamples()
        {
            return GenerateRandomEntityViewModel();
        }

        public static PiiEntityResponseContract GenerateRandomEntityViewModel()
        {
            lock (_lock)
            {
                var generateRandomEntityViewModel = _entityFaker.Generate();
                return generateRandomEntityViewModel;
            }
        }

        public static PiiIdsContract GetIDs(Faker f)
        {
            return new PiiIdsContract
            {
                MedicareId = f.Random.String2(10),
                MemberId = f.Random.String2(12)
            };
        }
    }
}