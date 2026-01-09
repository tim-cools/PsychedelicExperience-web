using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisationInfo : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }

        public RemoveOrganisationInfo(OrganisationId organisationId, UserId userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }
}