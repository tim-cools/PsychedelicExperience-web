using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FastMember;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;
using PsychedelicExperience.Psychedelics.NotificationView.Notifications;
using PrivacyLevel = PsychedelicExperience.Psychedelics.Messages.Experiences.PrivacyLevel;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class TopicStatusProjection : EventProjection
    {
        public override Type ViewType => typeof(NotificationTopicStatus);

        public TopicStatusProjection()
        {
            Configure();
        }

        private void Configure()
        {
            ConfigureUser();
            ConfigureExperience();

            ConfigureOrganisation();
            ConfigureOrganisationReview();
            ConfigureOrganisationUpdate();

            ConfigureDocument();

            ConfigureEvent();

            ConfigureInteraction();
        }

        private void ConfigureUser()
        {
            HandleEvent<UserProfileCreated>(
                context => context.Create(TopicType.UserProfile, ownerId: (Guid) context.Event.UserProfileId)
                    .NotifyOwners(userId => new UserProfileCreatedNotification(userId, context)),
                @event => @event.UserProfileId);

            HandleEvent<UserToRoleAdded>(
                context => context.Create(TopicType.UserProfile)
                    .NotifyOwners(userId => new UserToRoleAddedNotification(userId, context)),
                @event => @event.UserToChangeId);
        }

        private void ConfigureExperience()
        {
            HandleEvent<ExperienceAdded>(
                context => context.Create(TopicType.Experience, context.Event.Title?.Value, (Guid)context.Event.UserId)
                    .NotifyOwners(userId => new ExperienceAddedNotification(userId, context)),
                @event => @event.ExperienceId);

            HandleEvent<ExperiencePrivacyLevelChanged>(
                context => context.NotifyOwners(PrivacyNotificationType(context)),
                @event => @event.ExperienceId);

            HandleEvent<ExperienceTitleChanged>(
                context => context.NameChanged(context.Event.Title?.Value),
                @event => @event.ExperienceId);

            RemoveEvent<ExperienceRemoved>(@event => @event.ExperienceId, 
                (guid, context) => new RemovedNotification((Guid) context.Event.UserId, context.Event.EventTimestamp, (Guid) context.Event.UserId, context.StatusView.Topic, context.StatusView.Name));
        }

        private void ConfigureOrganisation()
        {
            HandleEvent<OrganisationAdded>(
                context => context.Create(TopicType.Organisation, context.Event.Name?.Value)
                    .NotifyOwners(userId => new OrganisationAddedNotification(userId, context)),
                @event => @event.OrganisationId);

            HandleEvent<OrganisationNameChanged>(
                context => context.NameChanged(context.Event.Name?.Value)
                    .NotifyOwners(userId => new OrganisationNameChangedNotification(userId, context)),
                @event => @event.OrganisationId);

            HandleEvent<OrganisationOwnerAdded>(
                context => context.AddOwner(context.Event.OwnerId)
                    .NotifyOwners(userId => new OrganisationOwnerAddedNotification(userId, context)),
                @event => @event.OrganisationId);

            HandleEvent<OrganisationOwnerRemoved>(
                context => context.AddOwner(context.Event.OwnerId)
                    .NotifyOwners(userId => new OrganisationOwnerRemovedNotification(userId, context)),
                @event => @event.OrganisationId);

            RemoveEvent<OrganisationRemoved>(@event => @event.OrganisationId,
                (guid, context) => new RemovedNotification((Guid)context.Event.UserId, context.Event.EventTimestamp, (Guid)context.Event.UserId, context.StatusView.Topic, context.StatusView.Name));
        }

        private void ConfigureOrganisationReview()
        {
            HandleEvent<OrganisationReviewAdded>(
                context => context.CreateChild(TopicType.OrganisationReview, (Guid) context.Event.OrganisationId, context.Event.Name)
                    .NotifyParentOwners(@event => @event.OrganisationId, (userId, parentStatus) => new OrganisationReviewAddedNotification(userId, context, parentStatus)),
                @event => @event.OrganisationReviewId);

            HandleEvent<OrganisationReviewNameChanged>(
                context => context.NameChanged(context.Event.Name),
                @event => @event.OrganisationReviewId);

            RemoveEvent<OrganisationReviewRemoved>(@event => @event.OrganisationReviewId,
                (guid, context) => new RemovedNotification((Guid)context.Event.UserId, context.Event.EventTimestamp, (Guid)context.Event.UserId, context.StatusView.Topic, context.StatusView.Name));
        }

        private void ConfigureOrganisationUpdate()
        {
            HandleEvent<OrganisationUpdateAdded>(
                context => context.CreateChild(TopicType.OrganisationUpdate, (Guid) context.Event.OrganisationId, context.Event.Subject)
                    .NotifyParentFollowers(@event => @event.OrganisationId, (userId, parentStatus) => new OrganisationUpdateAddedNotification(userId, context, parentStatus)),
                @event => @event.UpdateId);

            HandleEvent<OrganisationUpdateSubjectChanged>(
                context => context.NameChanged(context.Event.Subject),
                @event => @event.UpdateId);

            HandleEvent<OrganisationUpdatePrivacyChanged>(
                context => context.NotifyOwners(OrganisationUpdatePrivacyChanged(context)),
                @event => @event.UpdateId);

            RemoveEvent<OrganisationUpdateRemoved>(@event => @event.UpdateId,
                (guid, context) => new RemovedNotification((Guid)context.Event.UserId, context.Event.EventTimestamp, (Guid)context.Event.UserId, context.StatusView.Topic, context.StatusView.Name));
        }

        private Func<Guid, Notification> OrganisationUpdatePrivacyChanged(Context<OrganisationUpdatePrivacyChanged> context)
        {
            return context.Event.Privacy == OrganisationUpdatePrivacy.MembersOnly
                ? userId => new OrganisationUpdatePrivacyMembersOnlyNotification(userId, context)
                : (Func<Guid, Notification>)(userId => new OrganisationUpdatePrivacyPublicNotification(userId, context));
        }

        private void ConfigureDocument()
        {
            HandleEvent<DocumentAdded>(
                context => context.Create(TopicType.Document)
                    .NotifyOwners(userId => new DocumentAddedNotification(userId, context)),
                @event => @event.DocumentId);

            HandleEvent<DocumentNameChanged>(
                context => context.NameChanged(context.Event.Name?.Value),
                @event => @event.DocumentId);

            HandleEvent<DocumentPublished>(
                context => context.NotifyOwners(userId => new DocumentPublishedNotification(userId, context)),
                @event => @event.DocumentId);

            HandleEvent<DocumentUnpublished>(
                context => context.NotifyOwners(userId => new DocumentUnpublishedNotification(userId, context)),
                @event => @event.DocumentId);

            RemoveEvent<DocumentRemoved>(@event => @event.DocumentId,
                (guid, context) => new RemovedNotification((Guid)context.Event.UserId, context.Event.EventTimestamp, (Guid)context.Event.UserId, context.StatusView.Topic, context.StatusView.Name));
        }

        private void ConfigureEvent()
        {
            HandleEvent<EventAdded>(
                context => context.CreateChild(TopicType.Event, (Guid) context.Event.OrganisationId, context.Event.Name?.Value)
                    .NotifyParentFollowers(@event => @event.OrganisationId, (userId, parentStatus) => new EventAddedNotification(userId, context, parentStatus)),
                @event => @event.EventId);

            HandleEvent<EventNameChanged>(
                context => context.NameChanged(context.Event.Name?.Value),
                @event => @event.EventId);

            RemoveEvent<EventRemoved>(@event => @event.EventId,
                (guid, context) => new RemovedNotification((Guid)context.Event.UserId, context.Event.EventTimestamp, (Guid)context.Event.UserId, context.StatusView.Topic, context.StatusView.Name));
        }

        private void ConfigureInteraction()
        {
            HandleEvent<Followed>(
                context => context.AddFollower(context.Event.UserId)
                    .NotifyOwners(userId => new FollowedNotification(userId, context)),
                @event => @event.TopicId);

            HandleEvent<Unfollowed>(
                context => context.RemoveFollower(context.Event.UserId),
                @event => @event.TopicId);

            HandleEvent<Liked>(context =>
                    context.AddFollower(context.Event.UserId)
                        .NotifyOwners(userId => new LikedNotification(userId, context)),
                @event => @event.TopicId);

            //HandleEvent<Disliked>(context => 
            //    context.AddFollower(context.Event.UserId)
            //        .AddNotification(NotificationType.Disliked, context.View.Owners), 
            //    @event => @event.TopicId);

            HandleEvent<LikeCompensated>(
                context => context.RemoveFollower(context.Event.UserId),
                @event => @event.TopicId);

            //HandleEvent<DislikeCompensated>(context => context.View.Followers.Remove((Guid)context.Event.UserId), @event => @event.TopicId);

            /* HandleEvent<Viewed>(
                context => context.NotifyOwners(userId => new ViewedNotification(userId, context)),
                @event => @event.TopicId);
                */

            HandleEvent<Commented>(
                context => context.NotifyOwners(userId => new CommentedNotification(userId, context)),
                @event => @event.TopicId);
        }

        private static Func<Guid, Notification> PrivacyNotificationType(Context<ExperiencePrivacyLevelChanged> context)
        {
            return context.Event.NewLevel == PrivacyLevel.Public
                ? userId => new ExperiencePublishedNotification(userId, context)
                : (Func <Guid, Notification>) (userId => new ExperienceUnpublishedNotification(userId, context));
        }

        private void HandleEvent<T>(Action<Context<T>> handler, Expression<Func<T, Object>> propertyNameExpression)
            where T : Event
        {
            var propertyName = propertyNameExpression.GetPropertyName();
            var typeAccessor = CreateTypeAccessor<T>(propertyName);

            EventAsync<T>(async (session, id, @event) =>
            {
                var property = typeAccessor[@event, propertyName];
                var idProperty = property as Id;
                if (idProperty == null)
                {
                    throw new InvalidOperationException($"{propertyName} value is not an Id, is {property.GetType()}");
                }

                var eventId = idProperty.Value;

                var view = await GetViewForUpdate(session, eventId, @event);
                var context = new Context<T>(session, view, @event);

                handler(context);
            });
        }

        private static TypeAccessor CreateTypeAccessor<T>(string name) where T : Event
        {
            var typeAccessor = TypeAccessor.Create(typeof(T));
            if (typeAccessor.GetMembers().FirstOrDefault(member => member.Name == name) == null)
            {
                throw new InvalidOperationException($"Event Type '{typeof(T).FullName}' has no property '{name}'");
            }
            return typeAccessor;
        }

        private void RemoveEvent<TEvent>(Expression<Func<TEvent, object>> getId, Func<Guid, Context<TEvent>, Notification> notificationFactory) where TEvent : Event
        {
            HandleEvent(context =>
            {
                context.Session.Delete<NotificationTopicStatus>(context.StatusView.Id);
                context.NotifyOwners(id => notificationFactory(id, context));
            }, getId);
        }

        private static async Task<NotificationTopicStatus> GetViewForUpdate(IDocumentSession session, Guid id, Event @event)
        {
            var view = await session.LoadAsync<NotificationTopicStatus>(id) ?? new NotificationTopicStatus { Id = id };
            session.Store(view);

            view.LastUpdated = @event.EventTimestamp;

            return view;
        }
    }
}