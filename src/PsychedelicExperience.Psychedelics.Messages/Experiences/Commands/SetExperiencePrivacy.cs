using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class SetExperiencePrivacy : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public PrivacyLevel Level { get; }

        public SetExperiencePrivacy(ExperienceId experienceId, UserId userId, PrivacyLevel level)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Level = level;
        }
    }
}