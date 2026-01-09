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
using OrganisationUpdatePrivacy = PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.OrganisationUpdatePrivacy;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.OrganisationUpdates
{
    public class WhenChangeOrganisationPrivacyAsOwner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _administratId = UserId.New(); private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Result _result;

        public WhenChangeOrganisationPrivacyAsOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationOwner(_administratId, _organisationId, _userId);

            _updateId = context.AddOrganisationUpdate(_userId, _organisationId, privacy: OrganisationUpdatePrivacy.Public);

            var query = new SetOrganisationUpdatePrivacy(_userId, _organisationId, _updateId, OrganisationUpdatePrivacy.MembersOnly);
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
                aggregate.Privacy.ShouldBe(Psychedelics.OrganisationUpdates.OrganisationUpdatePrivacy.MembersOnly);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_updateId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdatePrivacyChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Privacy.ShouldBe(OrganisationUpdatePrivacy.MembersOnly);
            });
        }
    }

    public class WhenChangeOrganisationPrivacyAsAdministrator : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratId = UserId.New(); private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Result _result;

        public WhenChangeOrganisationPrivacyAsAdministrator(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);

            _updateId = context.AddOrganisationUpdate(_administratId, _organisationId, privacy: OrganisationUpdatePrivacy.Public);

            var query = new SetOrganisationUpdatePrivacy(_administratId, _organisationId, _updateId, OrganisationUpdatePrivacy.MembersOnly);
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
                aggregate.Privacy.ShouldBe(Psychedelics.OrganisationUpdates.OrganisationUpdatePrivacy.MembersOnly);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_updateId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdatePrivacyChanged>();

                @event.UserId.ShouldBe(_administratId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Privacy.ShouldBe(OrganisationUpdatePrivacy.MembersOnly);
            });
        }
    }

    public class WhenChangeOrganisationPrivacyAsInvalidUser : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _administratId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Exception _exception;

        public WhenChangeOrganisationPrivacyAsInvalidUser(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);
            _updateId = context.AddOrganisationUpdate(_administratId, _organisationId, privacy: OrganisationUpdatePrivacy.Public);

            var query = new SetOrganisationUpdatePrivacy(_userId, _organisationId, _updateId, OrganisationUpdatePrivacy.MembersOnly);

            _exception = Expect.Exception(() => context.Service.ExecuteNowWithTimeout(query));
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _exception.ShouldNotBeNull();
        }
    }
}