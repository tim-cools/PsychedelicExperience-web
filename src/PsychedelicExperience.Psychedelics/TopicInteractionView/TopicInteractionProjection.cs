using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FastMember;
using Marten;
using MemBus;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;
using PsychedelicExperience.Psychedelics.TopicInteractionView.Events;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView
{
    public class TopicInteractionProjection : EventProjection
    {
        private readonly IBus _bus;

        public override Type ViewType => typeof(TopicInteraction);

        public TopicInteractionProjection(IBus bus)
        {
            _bus = bus;

            Configure();
        }

        private void Configure()
        {
            TopicInteractionEventAsync<ExperienceAdded>(Project, nameof(ExperienceAdded.ExperienceId));
            TopicInteractionEventAsync<ExperiencePrivacyLevelChanged>(Project, nameof(ExperiencePrivacyLevelChanged.ExperienceId));
            RemoveEventAsync<ExperienceRemoved>(@event => (Guid) @event.ExperienceId);

            TopicInteractionEventAsync<OrganisationAdded>(Project, nameof(OrganisationAdded.OrganisationId));
            TopicInteractionEventAsync<OrganisationOwnerAdded>(Project, nameof(OrganisationOwnerAdded.OrganisationId));
            TopicInteractionEventAsync<OrganisationOwnerRemoved>(Project, nameof(OrganisationOwnerRemoved.OrganisationId));
            RemoveEventAsync<OrganisationRemoved>(@event => (Guid)@event.OrganisationId);

            TopicInteractionEventAsync<OrganisationReviewAdded>(Project, nameof(OrganisationReviewAdded.OrganisationReviewId));
            RemoveEventAsync<OrganisationReviewRemoved>(@event => (Guid)@event.OrganisationReviewId);

            TopicInteractionEventAsync<DocumentAdded>(Project, nameof(DocumentAdded.DocumentId));
            TopicInteractionEventAsync<DocumentPublished>(Project, nameof(DocumentPublished.DocumentId));
            TopicInteractionEventAsync<DocumentUnpublished>(Project, nameof(DocumentUnpublished.DocumentId));
            RemoveEventAsync<DocumentRemoved>(@event => (Guid)@event.DocumentId);

            TopicInteractionEventAsync<OrganisationUpdateAdded>(Project, nameof(OrganisationUpdateAdded.UpdateId));
            TopicInteractionEventAsync<OrganisationUpdatePrivacyChanged>(Project, nameof(OrganisationUpdatePrivacyChanged.UpdateId));
            RemoveEventAsync<OrganisationUpdateRemoved>(@event => (Guid)@event.UpdateId);

            TopicInteractionEventAsync<EventAdded>(Project, nameof(EventAdded.EventId));
            RemoveEventAsync<EventRemoved>(@event => (Guid)@event.EventId);

            TopicInteractionEventAsync<Followed>(Project);
            TopicInteractionEventAsync<Unfollowed>(Project);

            TopicInteractionEventAsync<Liked>(Project);
            TopicInteractionEventAsync<Disliked>(Project);

            TopicInteractionEventAsync<LikeCompensated>(Project);
            TopicInteractionEventAsync<DislikeCompensated>(Project);

            TopicInteractionEventAsync<Viewed>(Project);
            TopicInteractionEventAsync<Commented>(Project);
        }

        private void TopicInteractionEventAsync<T>(Action<TopicInteraction, T> handler, string propertyName = "TopicId") where T : Event
        {
            var typeAccessor = CreateTypeAccessor<T>(propertyName);

            EventAsync<T>(async (session, id, @event) =>
            {
                var property = typeAccessor[@event, propertyName];
                var idProperty = property as Id;
                if (idProperty == null)
                {
                    //"{propertyName} value is not an Id, is {property.GetType()}");
                    return;
                }

                var eventId = idProperty.Value;

                var view = await GetView(session, eventId, @event);

                handler(view, @event);

                _bus.Publish(new TopicInteractionUpdated(view));
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

        private void Project(TopicInteraction view, ExperienceAdded @event)
        {
            view.InteractionType = InteractionType.Experience;
            view.Owners.Add((Guid)@event.UserId);
        }

        private static void Project(TopicInteraction view, ExperiencePrivacyLevelChanged @event) => view.Public = @event.NewLevel == PrivacyLevel.Public;

        private static void Project(TopicInteraction view, OrganisationAdded @event) => view.InteractionType = InteractionType.Organisation;
        private static void Project(TopicInteraction view, OrganisationOwnerAdded @event) => view.Owners.Add((Guid)@event.OwnerId);
        private static void Project(TopicInteraction view, OrganisationOwnerRemoved @event) => view.Owners.Remove((Guid)@event.OwnerId);

        private static void Project(TopicInteraction view, OrganisationReviewAdded @event) => view.InteractionType = InteractionType.OrganisationReview;

        private static void Project(TopicInteraction view, DocumentAdded @event) => view.InteractionType = InteractionType.Document;
        private static void Project(TopicInteraction view, DocumentPublished @event) => view.Public = true;
        private static void Project(TopicInteraction view, DocumentUnpublished @event) => view.Public = false;

        private static void Project(TopicInteraction view, OrganisationUpdateAdded @event)
        {
            view.InteractionType = InteractionType.OrganisationUpdate;
            view.Public = @event.Privacy == OrganisationUpdatePrivacy.Public;
        }
        private static void Project(TopicInteraction view, OrganisationUpdatePrivacyChanged @event) => view.Public = @event.Privacy == OrganisationUpdatePrivacy.Public;

        private static void Project(TopicInteraction view, EventAdded @event)
        {
            view.InteractionType = InteractionType.Event;
            view.Public = @event.Privacy == EventPrivacy.Public;
        }

        private static void Project(TopicInteraction view, Followed @event) => view.Followers += 1;
        private static void Project(TopicInteraction view, Unfollowed @event) => view.Followers -= 1;

        private static void Project(TopicInteraction view, Liked @event) => view.Likes += 1;
        private static void Project(TopicInteraction view, Disliked @event) => view.Dislikes += 1;

        private static void Project(TopicInteraction view, LikeCompensated @event) => view.Likes -= 1;
        private static void Project(TopicInteraction view, DislikeCompensated @event) => view.Dislikes -= 1;

        private static void Project(TopicInteraction view, Viewed @event) => view.Views += 1;

        private void RemoveEventAsync<TEvent>(Func<TEvent, Guid> getId) where TEvent : Event
        {
            EventAsync<TEvent>((session, id, @event) =>
            {
                session.Delete<TopicInteraction>(getId(@event));
                return Task.CompletedTask;
            });
        }

        private void Project(TopicInteraction view, Commented @event)
        {
            if (view.Comments == null) view.Comments = new List<TopicComment>();

            view.Comments.Add(new TopicComment
            {
                UserId = @event.UserId,
                Timestamp = @event.EventTimestamp,
                Text = @event.Text
            });
        }

        private static async Task<TopicInteraction> GetView(IDocumentSession session, Guid id, Event @event)
        {
            var view = await session.LoadAsync<TopicInteraction>(id) ?? new TopicInteraction { Id = id };
            session.Store(view);

            view.LastUpdated = @event.EventTimestamp;

            return view;
        }
    }
}