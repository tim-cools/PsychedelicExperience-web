using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Events
{
    public class EventPrivacyChanged : Event
    {
        public UserId UserId { get; set; }
        public EventId EventId { get; set; }
        public EventPrivacy Privacy { get; set; }
    }
}