using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class RemoveExperienceTag : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public Name Tag { get; }

        public RemoveExperienceTag(ExperienceId experienceId, UserId userId, Name tag)
        {
            ExperienceId = experienceId;
            UserId = userId;
            Tag = tag;
        }
    }
}