using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventImageChanged : Event
    {
        public EventId EventId { get; set; }
        public UserId UserId { get; set; }
        public Image Image { get; set; }
    }
}