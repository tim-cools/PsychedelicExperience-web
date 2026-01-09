using System.Collections.Generic;
using System.Linq;
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
    public class WhenGettingOrganisationUpdates : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly List<OrganisationUpdateId> _ids = new List<OrganisationUpdateId>();

        private OrganisationUpdatesResult _result;
        private OrganisationId _organisationId;

        public WhenGettingOrganisationUpdates(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            _organisationId = context.AddOrganisation(_userId, "What else 1");

            _ids.Add(context.AddOrganisationUpdate(_userId, _organisationId, "subject 1"));
            _ids.Add(context.AddOrganisationUpdate(_userId, _organisationId, "subject 2"));
            _ids.Add(context.AddOrganisationUpdate(_userId, _organisationId, "subject 3"));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetOrganisationUpdates(_userId, _organisationId, true);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenOrganisationsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Updates.Length.ShouldBe(3);
            _result.Updates
                .Select(result => new OrganisationUpdateId(result.OrganisationUpdateId))
                .ShouldBeSubsetOf(_ids);
        }
    }
}
