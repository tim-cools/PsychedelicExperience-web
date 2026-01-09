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
    public class WhenAddOrganisationTag : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationTag(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationTag(_organisationId, _userId, "Retreat");
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
                aggregate.Tags.ShouldContain(tag => tag.Name == "Retreat");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationTagAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.TagName.ShouldBe("Retreat");
            });
        }
    }

    public class WhenAddOrganisationAdminTagAsAdmin : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationAdminTagAsAdmin(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationTag(_organisationId, _userId, "Psychedelic Experience");
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
                aggregate.Tags.ShouldContain(tag => tag.Name == "Psychedelic Experience");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationTagAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.TagName.ShouldBe("Psychedelic Experience");
            });
        }
    }


    public class WhenAddOrganisationAdminTagAsOwner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationAdminTagAsOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            var adminId = UserId.New();
            context.AddAdministrator(adminId);
            context.AddUser(_userId);

            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationOwner(adminId, _organisationId, _userId);

            var query = new AddOrganisationTag(_organisationId, _userId, "Psychedelic Experience");
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldFail()
        {
            _result.Succeeded.ShouldBe(false);
        }

        [Fact]
        public void ThenTheAggregateShouldNotBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Organisation>(_organisationId);

                aggregate.ShouldNotBeNull();
                aggregate.Tags.ShouldBeEmpty();
            });
        }
    }
}