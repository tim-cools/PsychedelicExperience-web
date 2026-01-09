using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Organisations;
using PsychedelicExperience.Membership.Tests.Integration;
using Xunit;
using PsychedelicExperience.Common.Tests.Messages;
using System.Linq;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using Shouldly;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class WhenChangeOrganisationAddress : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenChangeOrganisationAddress(PsychedelicsIntegrationTestFixture fixture)
              : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);

            _organisationId = context.AddOrganisation(_userId);

            var query = new ChangeOrganisationAddress(_organisationId, _userId, new Address("SomeAddress", null, null, "1233", null));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Organisation>(_organisationId);

                aggregate.ShouldNotBeNull();
                aggregate.Address.Name.ShouldBe("SomeAddress");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {

                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationAddressChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationId.ShouldBe(_organisationId);

            });
        }
    }
}