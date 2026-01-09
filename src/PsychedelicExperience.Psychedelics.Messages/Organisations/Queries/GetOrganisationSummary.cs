using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisationSummary : IRequest<OrganisationSummary>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }

        public GetOrganisationSummary(UserId userId, OrganisationId organisationId)
        {
            UserId = userId;
            OrganisationId = organisationId;
        }
    }
}