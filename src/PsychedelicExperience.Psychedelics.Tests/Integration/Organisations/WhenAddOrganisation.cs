using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
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
    public class WhenAddOrganisation : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;

        public WhenAddOrganisation(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            var query = new AddOrganisation(_organisationId, _userId, false, new string[] {}, new Name("name"), "This is a fancy ", null, new string[] { }, address, null, null, null, null);

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
                aggregate.Person.ShouldBeFalse();
                aggregate.Owners.ShouldBe(new Guid[] { });
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.Person.ShouldBeFalse();
                aggregate.Types.Count.ShouldBe(0);
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
                @event.Name.ShouldBe(new Name("name"));
                @event.Person.ShouldBeFalse();
                @event.Types.Length.ShouldBe(0);
            });
        }
    }

    public class WhenAddPerson : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;

        public WhenAddPerson(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            var query = new AddOrganisation(_organisationId, _userId, true, new string[] { "Facilitator" }, new Name("name"), "This is a fancy ", null, new string[] { }, address, null, null, null, null);

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
                aggregate.Person.ShouldBeTrue();
                aggregate.Owners.ShouldBe(new Guid[] { });
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.Types.ShouldContain("Facilitator");
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
                @event.Name.ShouldBe(new Name("name"));
                @event.Person.ShouldBeTrue();
                @event.Types.ShouldContain("Facilitator");
            });
        }
    }

    public class WhenAddOrganisationWithType : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;

        public WhenAddOrganisationWithType(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            var query = new AddOrganisation(_organisationId, _userId, false, new [] { "Retreat" }, new Name("name"), "This is a fancy ", null, new string[] { }, address, null, null, null, null);

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
                aggregate.Owners.ShouldBe(new Guid[] { });
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.Person.ShouldBeFalse();
                aggregate.Types.ShouldContain("Retreat");
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
                @event.Name.ShouldBe(new Name("name"));
                @event.Person.ShouldBeFalse();
                @event.Types.ShouldContain("Retreat");
            });
        }
    }

    public class WhenAddOrganisationAsCenter : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;
        private Center _center;

        public WhenAddOrganisationAsCenter(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            _center = TestData.Center();
            var query = new AddOrganisation(_organisationId, _userId, false, new [] { "Retreat" }, new Name("name"), "This is a fancy ", null, new string[] { }, address, _center, null, null, null);

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
                aggregate.Owners.ShouldBe(new List<Guid>());
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.Center.ShouldBeDeepEqual(_center);
                aggregate.Person.ShouldBeFalse();
                aggregate.Types.ShouldContain("Retreat");
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
                @event.Name.ShouldBe(new Name("name"));
                @event.Center.ShouldBeDeepEqual(_center);
                @event.Person.ShouldBeFalse();
                @event.Types.ShouldContain("Retreat");
            });
        }
    }

    public class WhenAddOrganisationAsPractitioner : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;
        private Practitioner _practitioner;

        public WhenAddOrganisationAsPractitioner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            _practitioner = TestData.Practitioner();
            var query = new AddOrganisation(_organisationId, _userId, false, new [] { "Retreat" }, new Name("name"), "This is a fancy ", null, new string[] { }, address,  null, null, null, _practitioner);

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
                aggregate.Owners.ShouldBe(new List<Guid>());
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.Practitioner.ShouldBeDeepEqual(_practitioner);
                aggregate.Person.ShouldBeFalse();
                aggregate.Types.ShouldContain("Retreat");
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
                @event.Name.ShouldBe(new Name("name"));
                @event.Practitioner.ShouldBeDeepEqual(_practitioner);
                @event.Person.ShouldBeFalse();
                @event.Types.ShouldContain("Retreat");
            });
        }
    }

    public class WhenAddOrganisationAsCommunity : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;
        private Community _community;

        public WhenAddOrganisationAsCommunity(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            _community = TestData.Community();
            var query = new AddOrganisation(_organisationId, _userId, false, new [] { "Retreat" }, new Name("name"), "This is a fancy ", null, new string[] { }, address,  null, _community, null, null);

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
                aggregate.Owners.ShouldBe(new List<Guid>());
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.Community.ShouldBeDeepEqual(_community);
                aggregate.Person.ShouldBeFalse();
                aggregate.Types.ShouldContain("Retreat");
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
                @event.Name.ShouldBe(new Name("name"));
                @event.Community.ShouldBeDeepEqual(_community);
                @event.Person.ShouldBeFalse();
                @event.Types.ShouldContain("Retreat");
            });
        }
    }

    public class WhenAddOrganisationAsHealthcareProvider : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;
        private HealthcareProvider _healthcareProvider;

        public WhenAddOrganisationAsHealthcareProvider(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var address = new Address("name", "BE", new Location(1.0m, 2.0m), "123", null);
            _healthcareProvider = TestData.HealthcareProvider();
            var query = new AddOrganisation(_organisationId, _userId, false, new [] { "Retreat" }, new Name("name"), "This is a fancy ", null, new string[] { }, address, null, null, _healthcareProvider, null);

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
                aggregate.Owners.ShouldBe(new List<Guid>());
                aggregate.Name.ShouldBe(new Name("name"));
                aggregate.HealthcareProvider.ShouldBeDeepEqual(_healthcareProvider);
                aggregate.Person.ShouldBeFalse();
                aggregate.Types.ShouldContain("Retreat");
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
                @event.Name.ShouldBe(new Name("name"));
                @event.HealthcareProvider.ShouldBeDeepEqual(_healthcareProvider);
                @event.Person.ShouldBeFalse();
                @event.Types.ShouldContain("Retreat");
            });
        }
    }

    public class WhenAddOrganisationInvalidName : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationId _organisationId = OrganisationId.New();
        private Result _result;

        public WhenAddOrganisationInvalidName(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);

            var query = new AddOrganisation(_organisationId, _userId, false, new [] { "retreat" }, new Name("n"), null, null, null, null, null, null, null, null);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldNotBeNull();
            _result.Succeeded.ShouldBeFalse();
        }
    }
}