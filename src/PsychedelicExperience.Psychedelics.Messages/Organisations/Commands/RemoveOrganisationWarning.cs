using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisationWarning : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }

        public RemoveOrganisationWarning(OrganisationId organisationId, UserId userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }
}