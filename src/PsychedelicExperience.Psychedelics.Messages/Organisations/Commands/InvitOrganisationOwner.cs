using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class InvitOrganisationOwner : IRequest<AddOrganisationOwnerResult>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public EMail UserEmail { get; }

        public InvitOrganisationOwner(OrganisationId organisationId, UserId userId, EMail userEmail)
        {
            OrganisationId = organisationId;
            UserId = userId;
            UserEmail = userEmail;
        }
    }
}