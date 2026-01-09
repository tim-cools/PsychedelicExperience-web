using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Organisations;
using Shouldly;
using Xunit;
using PsychedelicExperience.Membership.Tests.Integration;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class WhenSetOrganisationWarning : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenSetOrganisationWarning(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new SetOrganisationWarning(_organisationId, _userId, "title", "content");
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
                aggregate.Warning.Title.ShouldBe("title");
                aggregate.Warning.Content.ShouldBe("content");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationWarningSet>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Title.ShouldBe("title");
                @event.Content.ShouldBe("content");
            });
        }
    }

    public class WhenRemoveOrganisationWarning : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenRemoveOrganisationWarning(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationWarning(_organisationId, _userId);

            var query = new RemoveOrganisationWarning(_organisationId, _userId);
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
                aggregate.Warning.ShouldBeNull();
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<OrganisationWarningRemoved>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
            });
        }
    }

    public class WhenSetOrganisationInfo : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenSetOrganisationInfo(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new SetOrganisationInfo(_organisationId, _userId, "title", "content");
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
                aggregate.Info.Title.ShouldBe("title");
                aggregate.Info.Content.ShouldBe("content");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationInfoSet>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Title.ShouldBe("title");
                @event.Content.ShouldBe("content");
            });
        }
    }

    public class WhenRemoveOrganisationInfo : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenRemoveOrganisationInfo(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationInfo(_organisationId, _userId);

            var query = new RemoveOrganisationInfo(_organisationId, _userId);
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
                aggregate.Info.ShouldBeNull();
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<OrganisationInfoRemoved>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
            });
        }
    }
}