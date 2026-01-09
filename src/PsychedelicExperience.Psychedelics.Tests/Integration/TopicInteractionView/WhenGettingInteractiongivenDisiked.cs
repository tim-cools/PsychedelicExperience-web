using System;
using Marten.Schema.Identity;
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
    public class WhenGettingInteractiongivenDisiked : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private TopicInteractionDetails _result;
        private TopicId _topicId;

        public WhenGettingInteractiongivenDisiked(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser();

            var id = context.AddOrganisation(_userId);

            _topicId = new TopicId((Guid) id);
            context.ShouldSucceed(new Dislike(_userId, _topicId));
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
        public void TheTheTopicShouldNotBeLiked()
        {
            _result.ShouldNotBeNull();
            _result.Likes.ShouldBe(0);
        }

        [Fact]
        public void ThenHasLikedShouldBetrue()
        {
            _result.ShouldNotBeNull();
            _result.HasLiked.ShouldBeFalse();
        }

        [Fact]
        public void TheTheTopicShouldBeDisliked()
        {
            _result.ShouldNotBeNull();
            _result.Dislikes.ShouldBe(1);
        }

        [Fact]
        public void ThenHasDislikesShouldBetrue()
        {
            _result.ShouldNotBeNull();
            _result.HasDisliked.ShouldBeTrue();
        }
    }
}