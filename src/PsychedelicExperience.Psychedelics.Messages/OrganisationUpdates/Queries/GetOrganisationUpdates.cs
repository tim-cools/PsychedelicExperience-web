using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries
{
    public class GetOrganisationUpdates : IRequest<OrganisationUpdatesResult>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public int Page { get; }
        public bool IncludeOrganisation { get; }

        public GetOrganisationUpdates(UserId userId, OrganisationId id, bool includeOrganisation, int page = 0)
        {
            UserId = userId;
            OrganisationId = id;
            IncludeOrganisation = includeOrganisation;
            Page = page;
        }
    }    
}