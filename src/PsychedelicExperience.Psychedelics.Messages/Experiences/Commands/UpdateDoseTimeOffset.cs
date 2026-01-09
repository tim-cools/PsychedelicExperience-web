using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseTimeOffset : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public int? TimeOffsetSeconds { get; }

        public UpdateDoseTimeOffset(UserId userId, DoseId doseId, int? timeOffsetSeconds)
        {
            UserId = userId;
            DoseId = doseId;
            TimeOffsetSeconds = timeOffsetSeconds;
        }
    }
}