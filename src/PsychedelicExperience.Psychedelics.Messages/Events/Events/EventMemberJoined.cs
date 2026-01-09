using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventMemberJoined : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public UserId MemberId { get; set; }

        public EventMemberStatus Status { get; set; }
    }
}