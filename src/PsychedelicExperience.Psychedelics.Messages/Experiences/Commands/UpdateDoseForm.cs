using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseForm : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public string Form { get; }

        public UpdateDoseForm(UserId userId, DoseId doseId, string form)
        {
            UserId = userId;
            DoseId = doseId;
            Form = form;
        }
    }
}