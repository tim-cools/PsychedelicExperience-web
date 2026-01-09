using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries
{
    public class GetTopicFollowers : IRequest<TopicFollowersDetails>
    {
        public UserId UserId { get; }
        public TopicId TopicId { get; }

        public GetTopicFollowers(UserId userId, TopicId id)
        {
            UserId = userId;
            TopicId = id;
        }
    }
}