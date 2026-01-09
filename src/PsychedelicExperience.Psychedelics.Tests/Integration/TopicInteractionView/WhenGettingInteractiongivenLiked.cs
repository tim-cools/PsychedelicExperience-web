using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.TopicInteractionView
{
    public class WhenGettingInteractiongivenLiked : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractiongivenLiked(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();
            _topicId = TopicId.New();

            var id = context.AddOrganisation(_userId);

            _topicId = new TopicId((Guid)id);
            context.ShouldSucceed(new Like(_userId, _topicId));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetTopicInteraction(_userId, _topicId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void TheTheTopicShouldNotBeViewed()
        {
            _result.ShouldNotBeNull();
            _result.Views.ShouldBe(0);
        }

        [Fact]
        public void TheTheCommentsShouldBeEmpty()
        {
            _result.ShouldNotBeNull();
            _result.CommentCount.ShouldBe(0);
        }

        [Fact]
        public void TheTheTopicShouldBeLiked()
        {
            _result.ShouldNotBeNull();
            _result.Likes.ShouldBe(1);
        }

        [Fact]
        public void ThenHasLikedShouldBeTrue()
        {
            _result.ShouldNotBeNull();
            _result.HasLiked.ShouldBeTrue();
        }

        [Fact]
        public void TheTheTopicShouldNotBeDisliked()
        {
            _result.ShouldNotBeNull();
            _result.Dislikes.ShouldBe(0);
        }

        [Fact]
        public void ThenHasDislikesShouldBeFalse()
        {
            _result.ShouldNotBeNull();
            _result.HasDisliked.ShouldBeFalse();
        }
    }
}
