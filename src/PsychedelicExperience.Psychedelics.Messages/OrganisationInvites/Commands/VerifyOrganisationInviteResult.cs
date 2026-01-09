using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands
{
    public class VerifyOrganisationInviteResult : Result
    {
        public ShortGuid OrganisationId { get; }

        public VerifyOrganisationInviteResult()
        {
        }

        public VerifyOrganisationInviteResult(string errorcode, Guid? organisationId = null) : base(false, new ValidationError(null, errorcode, null))
        {
            OrganisationId = organisationId;
        }

        public VerifyOrganisationInviteResult(bool success, Guid? organisationId = null) : base(success)
        {
            OrganisationId = organisationId;
        }
    }
}