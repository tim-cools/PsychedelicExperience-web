using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class AddDose : IRequest<Result>
    {
        public ExperienceId ExperienceId { get; }
        public UserId UserId { get; }
        public DoseId DoseId { get; }

        public AddDose(ExperienceId experienceId, UserId userId, DoseId doseId)
        {
            ExperienceId = experienceId;
            UserId = userId;
            DoseId = doseId;
        }
    }
}