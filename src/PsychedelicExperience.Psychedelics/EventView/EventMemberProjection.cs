using System;
using Marten.Events.Projections;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;

namespace PsychedelicExperience.Psychedelics.EventView
{
    public class EventMemberProjection : ViewProjection<EventMember, Guid>
    {
        public EventMemberProjection()
        {
            ProjectEvent<EventMemberStatusChanged>(Project);
            ProjectEvent<EventMemberInvited>(Project);
            ProjectEvent<EventMemberJoined>(Project);
            ProjectEvent<EventMemberRemoved>(Project);
        }

        private void Project(EventMember view, EventMemberInvited @event)
        {
            view.EventId = (Guid)@event.EventId;
            view.MemberId = (Guid)@event.MemberId;
            view.Status = EventMemberStatus.Invited;
            view.AddedUserId = (Guid) @event.UserId;
        }

        private void Project(EventMember view, EventMemberJoined @event)
        {
            view.EventId = (Guid)@event.EventId;
            view.MemberId = (Guid)@event.MemberId;
            view.Status = @event.Status.CastByName<EventMemberStatus>();
            view.AddedUserId = (Guid)@event.UserId;
        }

        private void Project(EventMember view, EventMemberStatusChanged @event)
        {
            view.Status = @event.Status.CastByName<EventMemberStatus>();
        }

        private void Project(EventMember view, EventMemberRemoved @event)
        {
            view.Status = EventMemberStatus.Removed;
        }
    }
}