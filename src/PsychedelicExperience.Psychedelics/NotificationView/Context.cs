using System;
using System.Collections.Generic;
using System.Linq;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class Context<TEvent> where TEvent : Event
    {
        public IDocumentSession Session { get; }
        public TEvent Event { get; }
        public NotificationTopicStatus StatusView { get; }

        public Context(IDocumentSession session, NotificationTopicStatus view, TEvent @event)
        {
            Session = session;
            StatusView = view;

            Event = @event;
        }

        private void Notify(Func<Guid, Notification> notificationFactory, IList<Guid> userIds)
        {
            if (userIds.Count == 0) return;

            var notifications = userIds.Distinct()
                .Select(notificationFactory)
                .ToArray();

            Session.Store(notifications);
        }

        public void NotifyOwners(Func<Guid, Notification> notificationFactory)
        {
            Notify(notificationFactory, StatusView.Owners);
        }

        public void NotifyParentOwners(Func<TEvent, Id> parentId, Func<Guid, NotificationTopicStatus, Notification> notificationFactory)
        {
            var id = parentId(Event);
            var parentView = GetParentView(id);

            Notify(userId => notificationFactory(userId, parentView), parentView.Owners);
        }

        private NotificationTopicStatus GetParentView(Id id)
        {
            return GetView(Session, (Guid)id);
        }

        public void NotifyParentFollowers(Func<TEvent, Id> parentId, Func<Guid, NotificationTopicStatus, Notification> notificationFactory)
        {
            var id = parentId(Event);
            var parentView = GetView(Session, (Guid)id);
            var users = parentView.Owners.Union(parentView.Followers).Distinct().ToArray();

            Notify(userId => notificationFactory(userId, parentView), users);
        }

        public Context<TEvent> Create(TopicType topicType, string name = null, Guid? ownerId = null)
        {
            StatusView.Name = name;
            StatusView.Topic = new Topic
            {
                Id = StatusView.Id,
                TopicType = topicType
            };

            if (ownerId.HasValue)
            {
                StatusView.Owners.Add(ownerId.Value);
            }
            return this;
        }

        public Context<TEvent> CreateChild(TopicType topicType, Guid parentId, string name = null, Guid? ownerId = null)
        {
            StatusView.Name = name;
            StatusView.Topic = new Topic
            {
                Id = StatusView.Id,
                TopicType = topicType,
                ParentId = parentId
            };

            if (ownerId.HasValue)
            {
                StatusView.Owners.Add(ownerId.Value);
            }
            return this;
        }

        public Context<TEvent> NameChanged(string name)
        {
            StatusView.Name = name;
            return this;
        }

        public Context<TEvent> AddOwner(UserId userId)
        {
            StatusView.Owners.Add((Guid)userId);
            return this;
        }

        public Context<TEvent> AddFollower(UserId userId)
        {
            StatusView.Followers.Add((Guid)userId);
            return this;
        }

        public void RemoveFollower(UserId userId)
        {
            StatusView.Followers.Remove((Guid)userId);
        }

        private static NotificationTopicStatus GetView(IDocumentSession session, Guid id)
        {
            return session.Load<NotificationTopicStatus>(id) ?? new NotificationTopicStatus { Id = id };
        }
    }
}