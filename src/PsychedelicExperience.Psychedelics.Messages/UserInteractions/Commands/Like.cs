using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;

namespace PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands
{
    public class Like : IRequest<Result>
    {
        public UserId UserId { get; }
        public TopicId Id { get; }

        public Like(UserId userId, TopicId id)
        {
            UserId = userId;
            Id = id;
        }
    }
}