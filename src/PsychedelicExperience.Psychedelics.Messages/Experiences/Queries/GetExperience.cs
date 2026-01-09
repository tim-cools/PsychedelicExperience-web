using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class GetExperience : IRequest<ExperienceDetails>
    {
        public UserId UserId { get; }
        public ExperienceId ExperienceId { get; }

        public GetExperience(UserId userId, ExperienceId experienceId)
        {
            UserId = userId;
            ExperienceId = experienceId;
        }
    }
}
