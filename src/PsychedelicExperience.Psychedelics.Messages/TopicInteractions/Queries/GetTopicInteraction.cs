using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries
{
    public class GetTopicInteraction : IRequest<TopicInteractionDetails>
    {
        public UserId UserId { get; }
        public TopicId TopicId { get; }

        public GetTopicInteraction(UserId userId, TopicId topicId)
        {
            UserId = userId;
            TopicId = topicId;
        }
    }
}
