using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class RemoveDose : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }

        public RemoveDose(DoseId doseId, UserId userId)
        {
            UserId = userId;
            DoseId = doseId;
        }
    }
}