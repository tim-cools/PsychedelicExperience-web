using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseAmount : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public decimal? Amount { get; }

        public UpdateDoseAmount(UserId userId, DoseId doseId, decimal? amount)
        {
            UserId = userId;
            DoseId = doseId;
            Amount = amount;
        }
    }
}