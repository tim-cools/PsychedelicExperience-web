using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Organisations;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Events
{
    public class EventMember : AggregateRoot
    {
        private readonly  IList<Messages.Events.EventMemberStatus> _allowedChangeStatuses = new List<Messages.Events.EventMemberStatus>
        {
            Messages.Events.EventMemberStatus.Attending,
            Messages.Events.EventMemberStatus.NotAttending,
            Messages.Events.EventMemberStatus.Interested,
            Messages.Events.EventMemberStatus.Ignored
        };

        public Guid EventId { get; set; }
        public Guid MemberId { get; set; }

        public UserId AddedUserId { get; set; }

        public EventMemberStatus Status { get; set; }

        public void Handle(User user, InviteEventMember command)
        {
            EnsureNotJoined();

            Publish(new EventMemberInvited
            {
                UserId = new UserId(user.Id),
                EventId = command.EventId,
                MemberId = command.MemberId
            });
        }

        public void Handle(User user, JoinEventMember command)
        {
            EnsureNotJoined();

            Publish(new EventMemberJoined
            {
                UserId = new UserId(user.Id),
                EventId = command.EventId,
                MemberId = command.UserId,
                Status = command.Status,
            });
        }

        public void Handle(User user, ChangeEventMemberStatus command)
        {
            EnsureJoined();

            if (!_allowedChangeStatuses.Contains(command.Status))
            {
                throw new BusinessException("Invalid status: " + command.Status);
            }

            Publish(new EventMemberStatusChanged
            {
                UserId = new UserId(user.Id),
                EventId = (EventId)EventId,
                MemberId = (UserId)MemberId,
                Status = command.Status,
                PreviousStatus = Status.CastByName<Messages.Events.EventMemberStatus>()
            });
        }

        public void Handle(User user, RemoveEventMember command)
        {
            EnsureJoined();

            Publish(new EventMemberRemoved
            {
                UserId = new UserId(user.Id),
                EventId = (EventId) EventId,
                MemberId = (UserId) MemberId,
                PreviousStatus = Status.CastByName<Messages.Events.EventMemberStatus>()
            });
        }

        public void Apply(EventMemberInvited @event)
        {
            EventId = (Guid) @event.EventId;
            MemberId = (Guid) @event.MemberId;
            Status = EventMemberStatus.Invited;
            AddedUserId = @event.UserId;
        }

        public void Apply(EventMemberJoined @event)
        {
            EventId = (Guid) @event.EventId;
            MemberId = (Guid) @event.UserId;
            AddedUserId = @event.UserId;
            Status = @event.Status.CastByName<EventMemberStatus>();
        }

        public void Apply(EventMemberStatusChanged @event)
        {
            Status = @event.Status.CastByName<EventMemberStatus>();
        }

        public void Apply(EventMemberRemoved @event)
        {
            Status = EventMemberStatus.Removed;
        }

        private void EnsureJoined()
        {
            if (EventId == default(Guid))
            {
                throw new BusinessException("Member has not joined the event.");
            }
        }

        private void EnsureNotJoined()
        {
            if (EventId != default(Guid))
            {
                throw new BusinessException("Member already joined.");
            }
        }

        public void EnsureCanEdit(User user)
        {
            if (user == null || !user.IsAtLeast(Roles.ContentManager))
            {
                throw new BusinessException($"{user?.Id} could not edit event member {Id}!");
            }
        }
        public void EnsureCanAdd(User user, Organisation organisation)
        {
            if (user == null || !user.IsAtLeast(Roles.ContentManager) && !organisation.IsOwner(user))
            {
                throw new BusinessException($"could not add organisation event!");
            }
        }

        public void EnsureCanJoin(User user, Event @event, Organisation organisation)
        {
            EnsureOrganisation(@event, organisation);

            if (user == null)
            {
                throw new BusinessException($"User is null, could not join event {Id}!");
            }
        }

        internal void EnsureCanEdit(User user, Event @event, Organisation organisation)
        {
            EnsureOrganisation(@event, organisation);

            if (user == null || (MemberId != user.Id && !user.IsAtLeast(Roles.ContentManager) && !organisation.IsOwner(user)))
            {
                throw new BusinessException($"{user?.Id} could not edit event member {Id}!");
            }
        }

        public void EnsureCanRemove(User user, Event @event, Organisation organisation)
        {
            EnsureOrganisation(@event, organisation);

            if (user == null || (MemberId != user.Id && !user.IsAtLeast(Roles.ContentManager) && !organisation.IsOwner(user)))
            {
                throw new BusinessException($"{user?.Id} could not remove event member {Id}!");
            }
        }

        private void EnsureOrganisation(Event @event, Organisation organisation)
        {
            if (@event.OrganisationId != null && organisation != null && @event.OrganisationId.Value == organisation.Id) return;

            throw new BusinessException($@"Event {Id} does not belong to organisation {organisation?.Id}! ({@event.OrganisationId})");
        }
    }
}