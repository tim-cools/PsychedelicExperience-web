using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;

namespace PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events
{
    public class Unfollowed : Event
    {
        public UserInteractionId UserInteractionId { get; set; }
        public TopicId TopicId { get; set; }
        public UserId UserId { get; set; }
    }
}