using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseSubstance : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public string Substance { get; }

        public UpdateDoseSubstance(UserId userId, DoseId doseId, string substance)
        {
            UserId = userId;
            DoseId = doseId;
            Substance = substance;
        }
    }
}