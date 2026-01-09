using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventNameChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public Name Name { get; set; }

    }
}