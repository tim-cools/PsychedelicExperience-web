using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;

namespace PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events
{
    public class Commented : Event
    {
        public UserInteractionId UserInteractionId { get; set; }
        public TopicId TopicId { get; set; }
        public TopicId TopicType { get; set; }
        public UserId UserId { get; set; }
        public EncryptedString Text { get; set; }
    }
}