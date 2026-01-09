using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ConfirmOrganisationOwner : IRequest<AddOrganisationOwnerResult>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }

        public ConfirmOrganisationOwner(OrganisationId organisationId, UserId userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }
}