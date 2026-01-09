using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class WhenAddOrganisationReview : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationReviewId _organisationReviewId = OrganisationReviewId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationReview(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationReview(_organisationReviewId, 
                new DateTime(2000, 10, 12), 
                _organisationId, _userId, 
                "newName", "newDescription", ScaleOf5.Four, null, null,null, null, null);

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
                var aggregate = context.Session.Load<OrganisationReview>(_organisationReviewId);

                aggregate.ShouldNotBeNull();
                aggregate.Id.ShouldBe((Guid) _organisationReviewId);
                aggregate.OrganisationId.ShouldBe(_organisationId);
                aggregate.Owners.ShouldContain((Guid) _userId);
                aggregate.Name.ShouldBe("newName");
                aggregate.Description.ShouldBe("newDescription");
                aggregate.Rating.ShouldBe(ScaleOf5.Four);
                aggregate.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.UserId.ShouldBe(_userId);
                @event.Name.ShouldBe("newName");
                @event.Description.ShouldBe("newDescription");
                @event.Rating.ShouldBe(ScaleOf5.Four);
                @event.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }
    }

    public class WhenAddOrganisationReviewAsCenter : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationReviewId _organisationReviewId = OrganisationReviewId.New();
        private OrganisationId _organisationId;
        private Result _result;
        private CenterReview _center;

        public WhenAddOrganisationReviewAsCenter(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _center = TestData.CenterReview();

            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationReview(_organisationReviewId,
                new DateTime(2000, 10, 12),
                _organisationId, _userId,
                "newName", "newDescription", ScaleOf5.Four, _center, null, null, null, null);

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
                var aggregate = context.Session.Load<OrganisationReview>(_organisationReviewId);

                aggregate.ShouldNotBeNull();
                aggregate.Id.ShouldBe((Guid)_organisationReviewId);
                aggregate.OrganisationId.ShouldBe(_organisationId);
                aggregate.Owners.ShouldContain((Guid)_userId);
                aggregate.Name.ShouldBe("newName");
                aggregate.Description.ShouldBe("newDescription");
                aggregate.Rating.ShouldBe(ScaleOf5.Four);
                aggregate.Center.ShouldBeDeepEqual(_center);
                aggregate.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.UserId.ShouldBe(_userId);
                @event.Name.ShouldBe("newName");
                @event.Description.ShouldBe("newDescription");
                @event.Rating.ShouldBe(ScaleOf5.Four);
                @event.Center.ShouldBeDeepEqual(_center);
                @event.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }
    }

    public class WhenAddOrganisationReviewAsCommunity : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationReviewId _organisationReviewId = OrganisationReviewId.New();
        private OrganisationId _organisationId;
        private Result _result;
        private CommunityReview _community;

        public WhenAddOrganisationReviewAsCommunity(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _community = TestData.CommunityReview();

            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationReview(_organisationReviewId,
                new DateTime(2000, 10, 12),
                _organisationId, _userId,
                "newName", "newDescription", ScaleOf5.Four, null, _community, null, null, null);

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
                var aggregate = context.Session.Load<OrganisationReview>(_organisationReviewId);

                aggregate.ShouldNotBeNull();
                aggregate.Id.ShouldBe((Guid)_organisationReviewId);
                aggregate.OrganisationId.ShouldBe(_organisationId);
                aggregate.Owners.ShouldContain((Guid)_userId);
                aggregate.Name.ShouldBe("newName");
                aggregate.Description.ShouldBe("newDescription");
                aggregate.Rating.ShouldBe(ScaleOf5.Four);
                aggregate.Community.ShouldBeDeepEqual(_community);
                aggregate.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.UserId.ShouldBe(_userId);
                @event.Name.ShouldBe("newName");
                @event.Description.ShouldBe("newDescription");
                @event.Rating.ShouldBe(ScaleOf5.Four);
                @event.Community.ShouldBeDeepEqual(_community);
                @event.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }
    }

    public class WhenAddOrganisationReviewAsHealthcareProvider : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationReviewId _organisationReviewId = OrganisationReviewId.New();
        private OrganisationId _organisationId;
        private Result _result;
        private HealthcareProviderReview _healthcareProvider;

        public WhenAddOrganisationReviewAsHealthcareProvider(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _healthcareProvider = TestData.HealthcareProviderReview();

            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var query = new AddOrganisationReview(_organisationReviewId,
                new DateTime(2000, 10, 12),
                _organisationId, _userId,
                "newName", "newDescription", ScaleOf5.Four, null, null, _healthcareProvider, null, null);

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
                var aggregate = context.Session.Load<OrganisationReview>(_organisationReviewId);

                aggregate.ShouldNotBeNull();
                aggregate.Id.ShouldBe((Guid)_organisationReviewId);
                aggregate.OrganisationId.ShouldBe(_organisationId);
                aggregate.Owners.ShouldContain((Guid)_userId);
                aggregate.Name.ShouldBe("newName");
                aggregate.Description.ShouldBe("newDescription");
                aggregate.Rating.ShouldBe(ScaleOf5.Four);
                aggregate.HealthcareProvider.ShouldBeDeepEqual(_healthcareProvider);
                aggregate.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.UserId.ShouldBe(_userId);
                @event.Name.ShouldBe("newName");
                @event.Description.ShouldBe("newDescription");
                @event.Rating.ShouldBe(ScaleOf5.Four);
                @event.HealthcareProvider.ShouldBeDeepEqual(_healthcareProvider);
                @event.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }
    }

    public class WhenAddOrganisationReviewWithReview : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly OrganisationReviewId _organisationReviewId = OrganisationReviewId.New();
        private OrganisationId _organisationId;
        private Result _result;
        private ExperienceId _experienceId;

        public WhenAddOrganisationReviewWithReview(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);
            _experienceId = ExperienceId.New();

            var experience = new Experience(_experienceId, _userId, new Title("Experience"), new Description("Description"));
            var query = new AddOrganisationReview(_organisationReviewId,
                new DateTime(2000, 10, 12),
                _organisationId, _userId,
                "newName", "newDescription", ScaleOf5.Four, null, null, null, experience, "feedback");

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
                var aggregate = context.Session.Load<OrganisationReview>(_organisationReviewId);

                aggregate.ShouldNotBeNull();
                aggregate.Id.ShouldBe((Guid)_organisationReviewId);
                aggregate.OrganisationId.ShouldBe(_organisationId);
                aggregate.Owners.ShouldContain((Guid)_userId);
                aggregate.Name.ShouldBe("newName");
                aggregate.Description.ShouldBe("newDescription");
                aggregate.Rating.ShouldBe(ScaleOf5.Four);
                aggregate.ExperienceId.ShouldBeDeepEqual(_experienceId);
                aggregate.Visited.ShouldBe(new DateTime(2000, 10, 12));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.UserId.ShouldBe(_userId);
                @event.Name.ShouldBe("newName");
                @event.Description.ShouldBe("newDescription");
                @event.Rating.ShouldBe(ScaleOf5.Four);
                @event.ExperienceId.ShouldBeDeepEqual(_experienceId);
                @event.Visited.ShouldBe(new DateTime(2000, 10, 12));
                @event.Feedback.ShouldBe("feedback");
            });
        }

        [Fact]
        public void ThenTheExperienceShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Psychedelics.Experiences.Experience>(_experienceId);

                aggregate.ShouldNotBeNull();
                aggregate.Id.ShouldBe(_experienceId.Value);
                aggregate.Title.ShouldBe(new Title("Experience"));
                aggregate.Public.Description.ShouldBe(new Description("Description"));
            });
        }

        [Fact]
        public void ThenTheExperienceEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_experienceId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<ExperienceAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.Title.ShouldBe(new Title("Experience"));
                @event.Description.ShouldBe(new Description("Description"));
            });
        }
    }
}