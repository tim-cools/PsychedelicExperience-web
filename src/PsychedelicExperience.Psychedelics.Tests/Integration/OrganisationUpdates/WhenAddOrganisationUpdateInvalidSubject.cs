using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands;
using Shouldly;
using Xunit;
using OrganisationUpdatePrivacy = PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.OrganisationUpdatePrivacy;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationUpdates
{
    public class WhenAddOrganisationUpdateInvalidSubject : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratId = UserId.New();
        private readonly OrganisationUpdateId _updateId = OrganisationUpdateId.New();
        private Result _result;

        public WhenAddOrganisationUpdateInvalidSubject(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_administratId);

            var organisationId = context.AddOrganisation(_administratId);
            var query = new AddOrganisationUpdate(_administratId, organisationId, _updateId, "n",  "This is only a tests.", OrganisationUpdatePrivacy.Public);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
        }
    }
}