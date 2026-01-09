using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries
{
    public class GetOrganisationUpdate : IRequest<OrganisationUpdateResult>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationUpdateId UpdateId { get; }
        public bool IncludeOrganisation { get; }

        public GetOrganisationUpdate(UserId userId, OrganisationId id, OrganisationUpdateId updateId, bool includeOrganisation)
        {
            UserId = userId;
            OrganisationId = id;
            UpdateId = updateId;
            IncludeOrganisation = includeOrganisation;
        }
    }
}