using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.UserInteractions
{
    public class UserInteraction : AggregateRoot
    {
        public Guid TopicId { get; private set; }
        public Guid UserId { get; private set; }

        public bool Followed { get; private set; }
        public Opinion Opinion { get; private set; }

        public int Views { get; private set; }
        public int Comments { get; private set; }

        private void Add(UserInteractionId id, TopicId topicId, UserId userId)
        {
            Publish(new UserInteractionAdded
            {
                UserId = userId,
                TopicId = topicId,
                UserInteractionId = id
            });
        }

        public void Apply(UserInteractionAdded @event)
        {
            Id = (Guid)@event.UserInteractionId;
            TopicId = (Guid)@event.TopicId;
            UserId = (Guid)@event.UserId;
        }

        public void Follow(UserInteractionId id, TopicId topicId, UserId userId)
        {
            EnsureCreated(id, topicId, userId);

            if (Followed)
            {
                Publish(new Unfollowed
                {
                    UserId = userId,
                    TopicId = topicId,
                    UserInteractionId = id
                });
            }
            else
            {
                Publish(new Followed
                {
                    UserId = userId,
                    TopicId = topicId,
                    UserInteractionId = id
                });
            };
        }

        public void Apply(Followed @event)
        {
            Followed = true;
        }

        public void Apply(Unfollowed @event)
        {
            Followed = false;
        }

        public void Like(UserInteractionId id, TopicId topicId, UserId userId)
        {
            EnsureCreated(id, topicId, userId);

            if (Opinion == Opinion.Like)
            {
                Publish(new LikeCompensated
                {
                    UserId = userId,
                    TopicId = topicId,
                    UserInteractionId = id
                });
                return;
            }

            if (Opinion == Opinion.Dislike)
            {
                Publish(new DislikeCompensated
                {
                    UserId = userId,
                    TopicId = topicId,
                    UserInteractionId = id
                });
            }

            Publish(new Liked
            {
                UserId = userId,
                TopicId = topicId,
                UserInteractionId = id
            });
        }

        public void Dislike(UserInteractionId id, TopicId topicId, UserId userId)
        {
            EnsureCreated(id, topicId, userId);

            if (Opinion == Opinion.Dislike)
            {
                Publish(new DislikeCompensated
                {
                    UserId = userId,
                    TopicId = topicId,
                    UserInteractionId = id
                });
                return;
            }

            if (Opinion == Opinion.Like)
            {
                Publish(new LikeCompensated
                {
                    UserId = userId,
                    TopicId = topicId,
                    UserInteractionId = id
                });
            }

            Publish(new Disliked
            {
                UserId = userId,
                TopicId = topicId,
                UserInteractionId = id
            });
        }

        public void Apply(LikeCompensated @event)
        {
            Opinion = Opinion.None;
        }

        public void Apply(Liked @event)
        {
            Opinion = Opinion.Like;
        }

        public void Apply(DislikeCompensated @event)
        {
            Opinion = Opinion.None;
        }

        public void Apply(Disliked @event)
        {
            Opinion = Opinion.Dislike;
        }

        public void View(UserInteractionId id, TopicId topicId, UserId userId)
        {
            EnsureCreated(id, topicId, userId);

            Publish(new Viewed
            {
                UserId = userId,
                TopicId = topicId,
                UserInteractionId = id
            });
        }

        public void Apply(Viewed @event)
        {
            Id = (Guid) @event.UserInteractionId;
            TopicId = (Guid) @event.TopicId;
            UserId = (Guid) @event.UserId;

            Views++;
        }

        public void Comment(UserInteractionId id, TopicId topicId, UserId userId, EncryptedString text)
        {
            EnsureCreated(id, topicId, userId);

            Publish(new Commented
            {
                UserId = userId,
                TopicId = topicId,
                UserInteractionId = id,
                Text = text
            });
        }

        public void Apply(Commented @event)
        {
            Id = (Guid) @event.UserInteractionId;
            TopicId = (Guid) @event.TopicId;
            UserId = (Guid) @event.UserId;

            Comments ++;
        }

        private void EnsureCreated(UserInteractionId id, TopicId topicId, UserId userId)
        {
            if (IsNew())
            {
                Add(id, topicId, userId);
            }
        }

        private bool IsNew()
        {
            return TopicId == Guid.Empty;
        }
    }
}