using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisationOwner : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public UserId OwnerId { get; }

        public RemoveOrganisationOwner(OrganisationId organisationId, UserId userId, UserId ownerId)
        {
            OrganisationId = organisationId;
            UserId = userId;
            OwnerId = ownerId;
        }
    }
}