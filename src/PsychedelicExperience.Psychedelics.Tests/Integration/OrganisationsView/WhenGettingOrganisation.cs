using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;
using PsychedelicExperience.Membership.Tests.Integration;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationsView
{
    public class WhenGettingOrganisation : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private OrganisationDetails _result;
        private OrganisationId _organisationId;

        public WhenGettingOrganisation(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisation(_userId);
            context.AddOrganisation(_userId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetOrganisation(_userId, _organisationId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenOrganisationsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.OrganisationId.ShouldBe((ShortGuid) _organisationId.Value);
        }
    }


}
