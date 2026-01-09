using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class RemoveExperience : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }

        public RemoveExperience(ExperienceId experienceId, UserId userId)
        {
            ExperienceId = experienceId;
            UserId = userId;
        }
    }
}