using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventTypeChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public EventType EventType { get; set; }
    }
}