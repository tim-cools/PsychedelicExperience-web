using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventLocationChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public EventLocation Location { get; set; }

    }
}