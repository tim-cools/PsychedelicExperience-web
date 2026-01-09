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
    public class WhenAddOrganisationType : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationType(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationType(_organisationId, _userId, "Community");
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
                aggregate.Types.ShouldContain(tag => tag == "Community");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationTypeAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Type.ShouldBe("Community");
            });
        }
    }

    public class WhenAddOrganisationTypePerson : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationTypePerson(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);
            context.ChangeOrganisationPerson(_userId, _organisationId, true);

            var query = new AddOrganisationType(_organisationId, _userId, "Facilitator");
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
                aggregate.Types.Count.ShouldBe(1);
                aggregate.Types.ShouldContain(tag => tag == "Facilitator");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<OrganisationTypeAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Type.ShouldBe("Facilitator");
            });
        }
    }

    public class WhenAddOrganisationTypeNotPersonType : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Exception _exception;

        public WhenAddOrganisationTypeNotPersonType(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);
            context.ChangeOrganisationPerson(_userId, _organisationId, true);

            var query = new AddOrganisationType(_organisationId, _userId, "Community");
            _exception = Expect.Exception(() => context.Service.ExecuteNowWithTimeout(query));
        }


        [Fact]
        public void ThenTheCommandShouldFail()
        {
            _exception
                .ShouldBeOfType<AggregateException>()
                .InnerExceptions[0]
                .InnerException
                .Message.ShouldBe("Person can't have types: Community");
        }
    }

    public class WhenAddOrganisationTypeNotOrganisationType : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationTypeNotOrganisationType(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddAdministrator(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationType(_organisationId, _userId, "Shaman");
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldFail()
        {
            _result.Succeeded.ShouldBeFalse();
            _result.Errors.First().Message.ShouldBe("Invalid value: Shaman (Should be Retreat,Clinic,Research,Community,Education,Training Center,Shop,Business Services,Media,Coach,Therapist,Facilitator,Advocate,Scientist)");
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Organisation>(_organisationId);

                aggregate.ShouldNotBeNull();
                aggregate.Types.Count.ShouldBe(1);
                aggregate.Types[0].ShouldBe("Retreat");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
            });
        }
    }
}