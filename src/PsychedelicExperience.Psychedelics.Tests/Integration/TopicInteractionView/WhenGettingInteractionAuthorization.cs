using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.TopicInteractionView
{
    public class WhenGettingInteractionWithoutType : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionWithoutType(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _topicId = TopicId.New();

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_userId, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldBeNull()
        {
            _result.ShouldBeNull();
        }
    }

    public class WhenGettingInteractionGivenPrivateExperienceAndAdministrator : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private UserId _administratorId;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenPrivateExperienceAndAdministrator(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _administratorId = context.AddAdministrator();

            var id = context.AddExperience(_userId);
            _topicId = new TopicId((Guid) id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_administratorId, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }

    public class WhenGettingInteractionGivenPrivateExperienceAndOwner : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenPrivateExperienceAndOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();

            var id = context.AddExperience(_userId);
            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_userId, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }

    public class WhenGettingInteractionGivenPrivateExperienceAndOtherUser : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private UserId _otherUserIUd;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenPrivateExperienceAndOtherUser(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _otherUserIUd = context.AddUser();

            var id = context.AddExperience(_userId);
            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_otherUserIUd, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldBeNull();
        }
    }

    public class WhenGettingInteractionGivenPublicExperienceAndAdministrator : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private UserId _administratorId;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenPublicExperienceAndAdministrator(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _administratorId = context.AddAdministrator();

            var id = context.AddExperience(_userId);
            context.SetExperiencePrivacy(_userId, id, PrivacyLevel.Public);            

            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_administratorId, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }

    public class WhenGettingInteractionGivenPublicExperienceAndOwner : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenPublicExperienceAndOwner(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();

            var id = context.AddExperience(_userId);
            context.SetExperiencePrivacy(_userId, id, PrivacyLevel.Public);

            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_userId, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }

    public class WhenGettingInteractionGivenPublicExperienceAndOtherUser : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private UserId _otherUserIUd;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenPublicExperienceAndOtherUser(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _otherUserIUd = context.AddUser();

            var id = context.AddExperience(_userId);
            context.SetExperiencePrivacy(_userId, id, PrivacyLevel.Public);

            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_otherUserIUd, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }

    public class WhenGettingInteractionGivenOrganisation : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private UserId _otherUserIUd;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenOrganisation(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _otherUserIUd = context.AddUser();

            var id = context.AddOrganisation(_userId);
            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_otherUserIUd, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }

    public class WhenGettingInteractionGivenOrganisationReview : ServiceTestBase<IMediator>,
      IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private UserId _otherUserIUd;

        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractionGivenOrganisationReview(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _otherUserIUd = context.AddUser();

            var id = context.AddOrganisationReview(_userId);
            _topicId = new TopicId((Guid)id);

            context.ShouldSucceed(new Dislike(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_otherUserIUd, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeNull()
        {
            _result.ShouldNotBeNull();
        }
    }
}