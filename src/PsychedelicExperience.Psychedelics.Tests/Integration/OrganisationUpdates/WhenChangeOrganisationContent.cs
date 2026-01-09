using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;
using PsychedelicExperience.Psychedelics.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Tests.Integration.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationUpdates
{
    public class WhenChangeOrganisationContentAsOwner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _administratId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Result _result;
        private string _newContent = "this is the content";

        public WhenChangeOrganisationContentAsOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationOwner(_administratId, _organisationId, _userId);

            _updateId = context.AddOrganisationUpdate(_userId, _organisationId);

            var query = new SetOrganisationUpdateContent(_userId, _organisationId, _updateId, _newContent);
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
                var aggregate = context.Session.Load<OrganisationUpdate>(_updateId);

                aggregate.ShouldNotBeNull();
                aggregate.Content.ShouldBe(_newContent);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_updateId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdateContentChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Content.ShouldBe(_newContent);
            });
        }
    }

    public class WhenChangeOrganisationContentAsAdministrator : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Result _result;
        private string _newContent = "this is the content";

        public WhenChangeOrganisationContentAsAdministrator(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);
            _updateId = context.AddOrganisationUpdate(_administratId, _organisationId);

            var query = new SetOrganisationUpdateContent(_administratId, _organisationId, _updateId, _newContent);
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
                var aggregate = context.Session.Load<OrganisationUpdate>(_updateId);

                aggregate.ShouldNotBeNull();
                aggregate.Content.ShouldBe(_newContent);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_updateId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdateContentChanged>();

                @event.UserId.ShouldBe(_administratId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Content.ShouldBe(_newContent);
            });
        }
    }

    public class WhenChangeOrganisationContentAsInvalidUser : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratId = UserId.New();
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private string _newContent = "this is the content";
        private Exception _exception;

        public WhenChangeOrganisationContentAsInvalidUser(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);
            _updateId = context.AddOrganisationUpdate(_administratId, _organisationId);

            var query = new SetOrganisationUpdateContent(_userId, _organisationId, _updateId, _newContent);
            _exception = Expect.Exception(() => context.Service.ExecuteNowWithTimeout(query));
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _exception.ShouldNotBeNull();
        }
    }
}