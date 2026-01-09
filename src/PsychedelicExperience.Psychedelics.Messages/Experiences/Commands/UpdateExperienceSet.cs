using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateExperienceSet : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public Description Description { get; }

        public UpdateExperienceSet(ExperienceId experienceId, UserId userId, Description description)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Description = description;
        }
    }
}