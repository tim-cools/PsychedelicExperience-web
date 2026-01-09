using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateExperienceTitle : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public Title Title { get; }

        public UpdateExperienceTitle(ExperienceId experienceId, UserId userId, Title title)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Title = title;
        }
    }
}