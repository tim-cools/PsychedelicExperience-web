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
    public class WhenRateOrganisationReviewRated : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationReviewId _organisationReviewId;
        private Result _result;

        public WhenRateOrganisationReviewRated(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationReviewId = context.AddOrganisationReview(_userId, "review 1", "description  1", ScaleOf5.Four);

            var query = new RateOrganisationReview(_organisationReviewId, _userId, ScaleOf5.Two);
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
                aggregate.Rating.ShouldBe(ScaleOf5.Two);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewRated>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.Rating.ShouldBe(ScaleOf5.Two);
            });
        }
    }

    public class WhenChangeOrganisationReviewCenter : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private Result _result;
        private CenterReview _center;
        private OrganisationReviewId _organisationReviewId;

        public WhenChangeOrganisationReviewCenter(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationReviewId = context.AddOrganisationReview(_userId, "review 1", "description  1", ScaleOf5.Four);

            _center = TestData.CenterReview();

            var query = new ChangeOrganisationReviewCenter( _organisationReviewId, _userId, _center);
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
                aggregate.Center.ShouldBeDeepEqual(_center);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewCenterChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.Review.ShouldBeDeepEqual(_center);
            });
        }
    }

    public class WhenChangeOrganisationReviewCommunity : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationReviewId _organisationReviewId;
        private Result _result;
        private CommunityReview _community;

        public WhenChangeOrganisationReviewCommunity(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationReviewId = context.AddOrganisationReview(_userId, "review 1", "description  1", ScaleOf5.Four);

            _community = TestData.CommunityReview();

            var query = new ChangeOrganisationReviewCommunity(_organisationReviewId, _userId, _community);
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
                aggregate.Community.ShouldBeDeepEqual(_community);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewCommunityChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.Review.ShouldBeDeepEqual(_community);
            });
        }
    }

    public class WhenChangeOrganisationReviewHealthcareProvider : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private OrganisationReviewId _organisationReviewId;
        private Result _result;
        private HealthcareProviderReview _healthcareProvider;

        public WhenChangeOrganisationReviewHealthcareProvider(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationReviewId = context.AddOrganisationReview(_userId, "review 1", "description  1", ScaleOf5.Four);

            _healthcareProvider = TestData.HealthcareProviderReview();

            var query = new ChangeOrganisationReviewHealthcareProvider(_organisationReviewId, _userId, _healthcareProvider);
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
                aggregate.HealthcareProvider.ShouldBeDeepEqual(_healthcareProvider);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationReviewId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationReviewHealthcareProviderChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.OrganisationReviewId.ShouldBe(_organisationReviewId);
                @event.Review.ShouldBeDeepEqual(_healthcareProvider);
            });
        }
    }
}