using System;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView
{
    public class TopicFollowerProjection : EventProjection
    {
        public override Type ViewType => typeof(TopicFollower);

        public TopicFollowerProjection()
        {
            Configure();
        }

        private void Configure()
        {
            EventAsync<Followed>(Followed);
            EventAsync<Unfollowed>(Unfollowed);
        }

        private static Task Followed(IDocumentSession session, Guid id, Followed @event)
        {
            var userId = (Guid)@event.UserId;
            var topicId = (Guid)@event.TopicId;

            var follower = new TopicFollower
            {
                UserId = userId,
                TopicId = topicId,
                Since = @event.EventTimestamp
            };

            session.Store(follower);

            return Task.CompletedTask;
        }

        private static Task Unfollowed(IDocumentSession session, Guid id, Unfollowed @event)
        {
            var userId = (Guid)@event.UserId;
            var topicId = (Guid)@event.TopicId;

            session.DeleteWhere<TopicFollower>(follower => follower.UserId == userId && follower.TopicId == topicId);

            return Task.CompletedTask;
        }
    }
}