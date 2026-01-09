using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventReported : Event
    {
        public EventId EventId { get; set; }
        public UserId UserId { get; set; }
        public Description Reason { get; set; }
    }
}