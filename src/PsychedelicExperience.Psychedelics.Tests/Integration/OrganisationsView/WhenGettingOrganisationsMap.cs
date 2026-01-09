using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationsView
{
    public class WhenGettingOrganisationsMap : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly List<OrganisationId> _ids = new List<OrganisationId>();

        private OrganisationMapPoint[] _result;

        public WhenGettingOrganisationsMap(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddUser(_userId);

            _ids.Add(context.AddOrganisation(_userId, "What else 1"));
            _ids.Add(context.AddOrganisation(_userId, "What else 2"));
            _ids.Add(context.AddOrganisation(_userId, "What else 3"));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetOrganisationsMap(_userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenOrganisationsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            var point = _result.SingleOrDefault(value => value.Id.Guid == _ids[0].Value);
            point.ShouldNotBeNull();
            point.Position.Latitude.ShouldNotBe(0);
            point.Position.Longitude.ShouldNotBe(0);
        }
    }
}
