using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class WhenChangeOrganisationPerson : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenChangeOrganisationPerson(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            context
                .AddOrganisationType(_userId, _organisationId, "Retreat")
                .AddOrganisationType(_userId, _organisationId, "Clinic")
                .AddOrganisationType(_userId, _organisationId, "Community");

            var query = new ChangeOrganisationPerson(_organisationId, _userId, true);
            _result = context.Service.ExecuteNowWithTimeout(query);

            context
                .AddOrganisationType(_userId, _organisationId, "Coach")
                .AddOrganisationType(_userId, _organisationId, "Facilitator");
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
                aggregate.Types.Count.ShouldBe(2);
                aggregate.Types.ShouldContain("Coach");
                aggregate.Types.ShouldContain("Facilitator");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(6);

                var @event = events[3].Data.ShouldBeOfType<OrganisationPersonChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Person.ShouldBeTrue();
            });
        }
    }


    public class WhenChangeOrganisationPersonFalse : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenChangeOrganisationPersonFalse(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            context
                .ChangeOrganisationPerson(_userId, _organisationId, true)
                .AddOrganisationType(_userId, _organisationId, "Facilitator")
                .AddOrganisationType(_userId, _organisationId, "Therapist");

            var query = new ChangeOrganisationPerson(_organisationId, _userId, false);
            _result = context.Service.ExecuteNowWithTimeout(query);

            context
                .AddOrganisationType(_userId, _organisationId, "Retreat")
                .AddOrganisationType(_userId, _organisationId, "Clinic")
                .AddOrganisationType(_userId, _organisationId, "Community");
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
                aggregate.Types.Count.ShouldBe(3);
                aggregate.Types.ShouldContain("Retreat");
                aggregate.Types.ShouldContain("Clinic");
                aggregate.Types.ShouldContain("Community");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(8);

                var @event = events[4].Data.ShouldBeOfType<OrganisationPersonChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Person.ShouldBeFalse();
            });
        }
    }
}