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
    public class WhenChangeOrganisationSubjectAsOwner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _administratId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Result _result;
        private string _newSubject = "this is sthe subject";

        public WhenChangeOrganisationSubjectAsOwner(PsychedelicsIntegrationTestFixture fixture)
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

            var query = new SetOrganisationUpdateSubject(_userId, _organisationId, _updateId, _newSubject);
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
                aggregate.Subject.ShouldBe(_newSubject);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_updateId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdateSubjectChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Subject.ShouldBe(_newSubject);
            });
        }
    }

    public class WhenChangeOrganisationSubjectAsAdministrator : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Result _result;
        private string _newSubject = "this is sthe subject";

        public WhenChangeOrganisationSubjectAsAdministrator(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);
            _updateId = context.AddOrganisationUpdate(_administratId, _organisationId);

            var query = new SetOrganisationUpdateSubject(_administratId, _organisationId, _updateId, _newSubject);
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
                aggregate.Subject.ShouldBe(_newSubject);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_updateId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationUpdateSubjectChanged>();

                @event.UserId.ShouldBe(_administratId);

                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Subject.ShouldBe(_newSubject);
            });
        }
    }

    public class WhenChangeOrganisationSubjectAsInvalidUser : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _administratId = UserId.New();
        private OrganisationId _organisationId;
        private OrganisationUpdateId _updateId;
        private Exception _exception;
        private string _newSubject = "this is sthe subject";

        public WhenChangeOrganisationSubjectAsInvalidUser(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            context.AddAdministrator(_administratId);

            _organisationId = context.AddOrganisation(_administratId);
            _updateId = context.AddOrganisationUpdate(_administratId, _organisationId);

            var query = new SetOrganisationUpdateSubject(_userId, _organisationId, _updateId, _newSubject);

            _exception = Expect.Exception(() => context.Service.ExecuteNowWithTimeout(query));
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _exception.ShouldNotBeNull();
        }
    }

}