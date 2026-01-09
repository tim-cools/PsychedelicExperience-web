using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventTagRemoved : Event
    {
        public EventId EventId { get; set; }
        public UserId UserId { get; set; }
        public Name TagName { get; set; }
    }
}