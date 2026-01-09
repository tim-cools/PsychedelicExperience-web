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
    public class WhenAddOrganisationUpdateAsOwner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _administratId = UserId.New();
        private readonly OrganisationUpdateId _organisationUpdateId = OrganisationUpdateId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationUpdateAsOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationOwner(_administratId, _organisationId, _userId);

            var query = new AddOrganisationUpdate(_userId, _organisationId, _organisationUpdateId, "this", "is oonly a test!", OrganisationUpdatePrivacy.Public);

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
                var aggregate = context.Session.Load<OrganisationUpdate>(_organisationUpdateId);

                aggregate.ShouldNotBeNull();
                aggregate.Subject.ShouldBe("this");
                aggregate.Content.ShouldBe("is oonly a test!");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationUpdateId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdateAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Subject.ShouldBe("this");
                @event.Content.ShouldBe("is oonly a test!");
            });
        }
    }

    public class WhenAddOrganisationUpdateAsAdministrator : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratId = UserId.New();
        private readonly OrganisationUpdateId _organisationUpdateId = OrganisationUpdateId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationUpdateAsAdministrator(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);

            var query = new AddOrganisationUpdate(_administratId, _organisationId, _organisationUpdateId, "this", "is oonly a test!", OrganisationUpdatePrivacy.Public);

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
                var aggregate = context.Session.Load<OrganisationUpdate>(_organisationUpdateId);

                aggregate.ShouldNotBeNull();
                aggregate.Subject.ShouldBe("this");
                aggregate.Content.ShouldBe("is oonly a test!");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationUpdateId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdateAdded>();

                @event.UserId.ShouldBe(_administratId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Subject.ShouldBe("this");
                @event.Content.ShouldBe("is oonly a test!");
            });
        }
    }

    public class WhenAddOrganisationUpdateAsInvalidUser : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationUpdateId _organisationUpdateId = OrganisationUpdateId.New();
        private OrganisationId _organisationId;
        private Exception _exception;

        public WhenAddOrganisationUpdateAsInvalidUser(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationUpdate(_userId, _organisationId, _organisationUpdateId, "this", "is oonly a test!", OrganisationUpdatePrivacy.Public);

            _exception = Expect.Exception(() => context.Service.ExecuteNowWithTimeout(query));
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _exception.ShouldNotBeNull();
        }
    }
}