using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Hubspot.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Hubspot
{
    public class WhenCompare : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;

        public WhenCompare(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var command = new Compare(_userId);

            _result = context.Service.ExecuteNowWithTimeout(command);
        }

        [Fact(Skip = "Not a test")]
        public void ThenTheCommandShouldSucceed()
        {
            _result.Succeeded.ShouldBe(false);
        }
    }
}