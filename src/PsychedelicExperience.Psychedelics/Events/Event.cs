using System;
using System.Collections.Generic;
using System.Linq;
using Baseline;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Organisations;
using Image = PsychedelicExperience.Psychedelics.Messages.Events.Image;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Events
{
    public class Event : AggregateRoot
    {
        public UserId UserId { get; set; }

        public OrganisationId OrganisationId { get; set; }

        public bool Removed { get; set; }

        public EventPrivacy Privacy { get; set; }
        public EventType EventType { get; set; }

        public Name Name { get; set; }
        public Description Description { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public EventLocation Location { get; set; }
        public Image Image { get; set; }

        public IList<Tag> Tags { get; } = new List<Tag>();

        public void Handle(User user, AddEvent command)
        {
            Publish(new EventAdded
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                OrganisationId = command.OrganisationId,
                Privacy = command.Privacy,
                EventType = command.EventType,
                StartDateTime = command.StartDateTime,
                EndDateTime = command.EndDateTime,
                Location = command.Location,
                Name = command.Name,
                Description = command.Description,
                Tags = command.Tags
            });
        }

        public void Handle(User user, AddEventTag command)
        {
            Publish(new EventTagAdded
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                TagName = command.TagName,
            });
        }

        public void Handle(User user, ChangeEventStartDateTime command)
        {
            Publish(new EventStartDateTimeChanged
            {
                EventId = (EventId)Id,
                UserId = new UserId(user.Id),
                DateTime = command.DateTime,
            });
        }

        public void Handle(User user, ChangeEventEndDateTime command)
        {
            Publish(new EventEndDateTimeChanged
            {
                EventId = (EventId)Id,
                UserId = new UserId(user.Id),
                DateTime = command.DateTime,
            });
        }

        public void Handle(User user, ChangeEventDescription command)
        {
            Publish(new EventDescriptionChanged
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                Description = command.Description,
            });
        }

        public void Handle(User user, ChangeEventImage command)
        {
            Publish(new EventImageChanged
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                Image = command.Image,
            });
        }

        public void Handle(User user, ChangeEventLocation command)
        {
            Publish(new EventLocationChanged
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                Location = command.Location,
            });
        }

        public void Handle(User user, ChangeEventName command)
        {
            Publish(new EventNameChanged
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                Name = command.Name,
            });
        }

        public void Handle(User user, ChangeEventPrivacy command)
        {
            Publish(new EventPrivacyChanged
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                Privacy = command.Privacy,
            });
        }

        public void Handle(User user, ChangeEventType command)
        {
            Publish(new EventTypeChanged
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                EventType = command.EventType,
            });
        }

        public void Handle(User user, ClearEventImage command)
        {
            Publish(new EventImageCleared
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
            });
        }

        public void Handle(User user, RemoveEvent command)
        {
            Publish(new EventRemoved
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
            });
        }

        public void Handle(User user, RemoveEventTag command)
        {
            Publish(new EventTagRemoved
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                TagName = command.TagName,
            });
        }

        public void Handle(User user, ReportEvent command)
        {
            Publish(new EventReported
            {
                EventId = (EventId) Id,
                UserId = new UserId(user.Id),
                Reason = command.Reason,
            });
        }

        public void Apply(EventAdded @event)
        {
            UserId = @event.UserId;
            Privacy = @event.Privacy.CastByName<EventPrivacy> ();
            EventType = @event.EventType.CastByName<EventType>(); ;
            StartDateTime = @event.StartDateTime;
            Location = @event.Location;
            Name = @event.Name;
            if (@event.Tags != null)
            {
                Tags.AddRange(@event.Tags.Select(tag => new Tag(tag)));
            }
            Description = @event.Description;
            OrganisationId = @event.OrganisationId;
        }

        public void Apply(EventRemoved @event)
        {
            Removed = true;
        }

        public void Apply(EventStartDateTimeChanged @event)
        {
            StartDateTime = @event.DateTime;
        }

        public void Apply(EventEndDateTimeChanged @event)
        {
            EndDateTime = @event.DateTime;
        }

        public void Apply(EventLocationChanged @event)
        {
            Location = @event.Location;
        }

        public void Apply(EventNameChanged @event)
        {
            Name = @event.Name;
        }

        public void Apply(EventDescriptionChanged @event)
        {
            Description = @event.Description;
        }

        public void Apply(EventImageChanged @event)
        {
            Image = @event.Image;
        }

        public void Apply(EventImageCleared @event)
        {
        }

        public void Apply(EventPrivacyChanged @event)
        {
            Privacy = @event.Privacy.CastByName<EventPrivacy>();
        }

        public void Apply(EventTypeChanged @event)
        {
            EventType = @event.EventType.CastByName<EventType>();
        }

        public void Apply(EventTagAdded @event)
        {
            var tag = new Tag(@event.TagName);
            Tags.Add(tag);
        }

        public void Apply(EventTagRemoved @event)
        {
            var tag = Tags.Single(where => Equals(where.Name, @event.TagName));
            Tags.Remove(tag);
        }

        public void Apply(EventReported @event)
        {
        }

        public void EnsureCanAdd(User user, Organisation organisation)
        {
            if (user == null)
            {
                throw new BusinessException($"could not add organisation event!");
            }
        }

        public void EnsureCanEdit(User user, Organisation organisation)
        {
            EnsureOrganisation(organisation);

            if (user == null || !user.IsAtLeast(Roles.ContentManager) && !organisation.IsOwner(user))
            {
                throw new BusinessException($"{user?.Id} could not edit organisation event {Id}!");
            }
        }

        public void EnsureCanRemove(User user, Organisation organisation)
        {
            EnsureOrganisation(organisation);

            if (user == null || !user.IsAtLeast(Roles.ContentManager) && !organisation.IsOwner(user))
            {
                throw new BusinessException($"{user?.Id} could not remove organisation event {Id}!");
            }
        }

        private void EnsureOrganisation(Organisation organisation)
        {
            if (OrganisationId != null && organisation != null && OrganisationId.Value == organisation.Id) return;

            throw new BusinessException($@"Event {Id} does not belong to organisation {organisation?.Id}! ({OrganisationId})");
        }
    }
}