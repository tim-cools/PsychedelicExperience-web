using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseNotes : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public string Notes { get; }

        public UpdateDoseNotes(UserId userId, DoseId doseId, string notes)
        {
            UserId = userId;
            DoseId = doseId;
            Notes = notes;
        }
    }
}