using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class SetExperienceLevel : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public ExperienceLevel Level { get; }

        public SetExperienceLevel(ExperienceId experienceId, UserId userId, ExperienceLevel level)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Level = level;
        }
    }
}