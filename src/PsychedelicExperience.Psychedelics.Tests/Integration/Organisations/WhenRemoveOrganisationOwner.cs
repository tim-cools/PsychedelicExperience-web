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
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class WhenRemoveOrganisationOwner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _administratorId = UserId.New();
        private readonly UserId _userId = UserId.New();
        private readonly UserId _ownerId = UserId.New();
        private OrganisationId _organisationId;
        private AggregateException _exception;

        public WhenRemoveOrganisationOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_ownerId);
            context.AddUser(_userId);
            context.AddAdministrator(_administratorId);

            _organisationId = context.AddOrganisation(_administratorId);
            context.AddOrganisationOwner(_administratorId, _organisationId, _ownerId);

            var query = new RemoveOrganisationOwner(_organisationId, _userId, _ownerId);
            _exception = Assert.Throws<AggregateException>(() => context.Service.ExecuteNowWithTimeout(query));
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _exception.ShouldNotBeNull();
        }
    }

    public class WhenRemoveOrganisationOwnerByAdmin : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly UserId _ownerId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenRemoveOrganisationOwnerByAdmin(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            context.AddUser(_ownerId);

            _organisationId = context.AddOrganisation(_userId);
            context.AddOrganisationOwner(_userId, _organisationId, _ownerId);

            var query = new RemoveOrganisationOwner(_organisationId, _userId, _ownerId);
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
                aggregate.Owners.ShouldNotContain(_ownerId.Value);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<OrganisationOwnerRemoved>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.OwnerId.ShouldBe(_ownerId);
            });
        }
    }
}