using System;
using System.Linq;
using Baseline;
using Marten.Events.Projections;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;

namespace PsychedelicExperience.Psychedelics.EventView
{
    public class EventProjection : ViewProjection<Event, Guid>
    {
        public EventProjection()
        {
            ProjectEvent<EventAdded>(Project);
            ProjectEvent<EventRemoved>(Project);
            ProjectEvent<EventStartDateTimeChanged>(Project);
            ProjectEvent<EventEndDateTimeChanged>(Project);
            ProjectEvent<EventLocationChanged>(Project);
            ProjectEvent<EventNameChanged>(Project);
            ProjectEvent<EventDescriptionChanged>(Project);

            ProjectEvent<EventImageChanged>(Project);
            ProjectEvent<EventImageCleared>(Project);

            ProjectEvent<EventPrivacyChanged>(Project);
            ProjectEvent<EventTypeChanged>(Project);
       
            ProjectEvent<EventTagAdded>(Project);
            ProjectEvent<EventTagRemoved>(Project);

            ProjectEvent<EventMemberStatusChanged>(@event => (Guid) @event.EventId, Project);
            ProjectEvent<EventMemberInvited>(@event => (Guid)@event.EventId, Project);
            ProjectEvent<EventMemberJoined>(@event => (Guid)@event.EventId, Project);
            ProjectEvent<EventMemberRemoved>(@event => (Guid)@event.EventId, Project);

            //ProjectEvent<EventReported>(Project);
        }

        private void Project(Event view, EventAdded @event)
        {
            view.Created = @event.EventTimestamp;
            view.OrganisationId = (Guid) @event.OrganisationId;
            view.Name = (string) @event.Name;
            view.Tags.AddRange(@event.Tags.Select(tag => tag.Value));
            view.Description = (string) @event.Description;
            view.UserId = @event.UserId.Value;
            view.StartDateTime = @event.StartDateTime;
            view.EndDateTime = @event.EndDateTime;
            view.LocationName = @event.Location?.Name?.Value;
            view.Address = @event.Location?.Address != null ? new EventAddress
            {
                Name = @event.Location.Address.Name,
                Country = @event.Location.Address.Country,
                PlaceId = @event.Location.Address.PlaceId,
                Position = new Position
                {
                    Latitude = @event.Location.Address.Location.Latitude,
                    Longitude = @event.Location.Address.Location.Longitude,
                }
            } : null;
            view.EventType = @event.EventType.CastByName<EventType>();
            view.Privacy = @event.Privacy.CastByName<EventPrivacy>();
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventRemoved @event)
        {
            view.Removed = true;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventStartDateTimeChanged @event)
        {
            view.StartDateTime = @event.DateTime;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventEndDateTimeChanged @event)
        {
            view.EndDateTime = @event.DateTime;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventLocationChanged @event)
        {
            view.LocationName = (string) @event.Location?.Name;
            view.Address = CreateAddress(@event.Location?.Address);
            view.Country = @event.Location?.Address?.Country;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventImageChanged @event)
        {
            view.SetImage(@event.Image.Id.Value, @event.Image.FileName, @event.Image.OriginalFileName);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventImageCleared @event)
        {
            view.ClearImage();
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventPrivacyChanged @event)
        {
            view.Privacy = @event.Privacy.CastByName<EventPrivacy>();

            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventTypeChanged @event)
        {
            view.EventType = @event.EventType.CastByName<EventType>();

            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventNameChanged @event)
        {
            view.Name = @event.Name?.Value;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventDescriptionChanged @event)
        {
            view.Description = @event.Description?.Value;
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventTagAdded @event)
        {
            view.AddTag((string) @event.TagName);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventTagRemoved @event)
        {
            view.RemoveTag((string)@event.TagName);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventMemberInvited @event)
        {
            view.Members.Add(Messages.Events.EventMemberStatus.Invited);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventMemberJoined @event)
        {
            view.Members.Add(@event.Status);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventMemberStatusChanged @event)
        {
            view.Members.Add(@event.Status);
            view.Members.Remove(@event.PreviousStatus);
            view.Updated(@event, @event.UserId);
        }

        private void Project(Event view, EventMemberRemoved @event)
        {
            view.Members.Remove(@event.PreviousStatus);
            view.Updated(@event, @event.UserId);
        }

        private static EventAddress CreateAddress(Address address)
        {
            return address != null
                ? new EventAddress
                {
                    Name = address.Name,
                    Position = MapPosition(address),
                    Country = address.Country.NormalizeForSearch(),
                    PlaceId = address.PlaceId
                }
                : null;
        }

        private static Position MapPosition(Address address)
        {
            return address.Location != null
                ? new Position
                {
                    Latitude = address.Location.Latitude,
                    Longitude = address.Location.Longitude
                }
                : null;
        }
    }
}