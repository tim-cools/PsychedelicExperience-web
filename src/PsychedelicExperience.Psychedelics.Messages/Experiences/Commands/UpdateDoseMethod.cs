using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public class UpdateDoseMethod : IRequest<Result>
    {
        public UserId UserId { get; }
        public DoseId DoseId { get; }
        public string Method { get; }

        public UpdateDoseMethod(UserId userId, DoseId doseId, string method)
        {
            UserId = userId;
            DoseId = doseId;
            Method = method;
        }
    }
}