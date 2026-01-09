using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationUpdatesView
{
    public class WhenGettingOrganisationUpdate : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private OrganisationUpdateResult _result;
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;

        public WhenGettingOrganisationUpdate(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddAdministrator();
            _organisationId = context.AddOrganisation(_userId);
            _updateId = context.AddOrganisationUpdate(_userId, _organisationId);

            context.AddOrganisationUpdate(_userId, _organisationId);
            context.AddOrganisationUpdate(_userId, _organisationId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetOrganisationUpdate(_userId, _organisationId, _updateId, true);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenOrganisationsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Update.OrganisationId.ShouldBe((ShortGuid) _organisationId.Value);
        }
    }
}
