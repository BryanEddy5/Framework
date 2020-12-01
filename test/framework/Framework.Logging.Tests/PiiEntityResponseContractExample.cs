using System.Linq;
using AutoBogus;
using Bogus;
using Bogus.DataSets;
using Swashbuckle.AspNetCore.Filters;

namespace HumanaEdge.Webcore.Framework.Logging.Tests
{
    /// <summary>
    /// An implementation of Swashbuckle's <see cref="IExamplesProvider{T}"/> for <see cref="PiiEntityResponseContract"/>.
    /// </summary>
    public class PiiEntityResponseContractExample : IExamplesProvider<PiiEntityResponseContract>
    {
        private static readonly object Lock = new object();
        private static readonly Faker<PiiEntityResponseContract> EntityFaker;

        static PiiEntityResponseContractExample()
        {
            AutoFaker.Configure(builder => { builder.WithSkip<PiiEntityResponseContract>("EntityId"); });

            EntityFaker = new AutoFaker<PiiEntityResponseContract>()
                .RuleFor(e => e.BirthDate, f => PiiEntityDataGenerator.GetBirthDate(f).ToString("yyyy-MM-dd"))
                .RuleFor(e => e.Zipcode, PiiEntityDataGenerator.Get5DigitZipCode)
                .RuleFor(e => e.Gender, f => f.PickRandom<Name.Gender>().ToString())
                .RuleFor(
                    e => e.Phones,
                    f => PiiEntityDataGenerator.GetPhones(f).Select(phone => phone).ToList())
                .RuleFor(e => e.Roles, f => new PiiCoreContracts.PiiRolesContract<PiiCoreContracts.PiiRoleContract>
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

        /// <summary>
        /// Return an example of a <see cref="PiiEntityResponseContract"/>.
        /// </summary>
        /// <returns>An example of a <see cref="PiiEntityResponseContract"/>.</returns>
        public PiiEntityResponseContract GetExamples()
        {
            return GenerateRandomEntityViewModel();
        }

        /// <summary>
        /// Return an example of a <see cref="PiiEntityResponseContract"/>.
        /// </summary>
        /// <returns>An example of a <see cref="PiiEntityResponseContract"/>.</returns>
        private static PiiEntityResponseContract GenerateRandomEntityViewModel()
        {
            lock (Lock)
            {
                var generateRandomEntityViewModel = EntityFaker.Generate();
                return generateRandomEntityViewModel;
            }
        }

        /// <summary>
        /// Return an example of a <see cref="PiiCoreContracts.PiiIdsContract"/>.
        /// </summary>
        /// <param name="f"><see cref="Faker"/>.</param>
        /// <returns>An example of a <see cref="PiiCoreContracts.PiiIdsContract"/>.</returns>
        private static PiiCoreContracts.PiiIdsContract GetIDs(Faker f)
        {
            return new PiiCoreContracts.PiiIdsContract
            {
                MedicareId = f.Random.String2(10),
                MemberId = f.Random.String2(12)
            };
        }
    }
}