using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseUnit : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public string Unit { get; }

        public UpdateDoseUnit(UserId userId, DoseId doseId, string unit)
        {
            UserId = userId;
            DoseId = doseId;
            Unit = unit;
        }
    }
}