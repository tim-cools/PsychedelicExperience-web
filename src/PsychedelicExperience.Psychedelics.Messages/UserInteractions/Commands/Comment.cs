using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;

namespace PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands
{
    public class Comment : IRequest<Result>
    {
        public UserId UserId { get; }
        public TopicId Id { get; }
        public string Text { get; }

        public Comment(UserId userId, TopicId id, string text)
        {
            UserId = userId;
            Id = id;
            Text = text;
        }
    }
}