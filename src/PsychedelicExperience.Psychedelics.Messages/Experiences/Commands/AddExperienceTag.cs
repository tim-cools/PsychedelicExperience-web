using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class AddExperienceTag : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public Name TagName { get; }

        public AddExperienceTag(ExperienceId experienceId, UserId userId, Name tagName)
        {
            ExperienceId = experienceId;
            UserId = userId;
            TagName = tagName;
        }
    }
}