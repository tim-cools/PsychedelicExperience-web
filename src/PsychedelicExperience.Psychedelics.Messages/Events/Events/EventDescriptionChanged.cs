using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventDescriptionChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public Description Description { get; set; }

    }
}