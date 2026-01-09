using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands
{
    public class ConfirmOrganisationInviteResult : Result
    {
        public ConfirmOrganisationInviteResult()
        {
        }

        public ConfirmOrganisationInviteResult(bool success, string errorcode) : base(success, new ValidationError(null, errorcode, null))
        {
        }

        public ConfirmOrganisationInviteResult(bool success) : base(success)
        {
        }
    }
}