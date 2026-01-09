using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationCommunity : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public Community Community { get; }

        public ChangeOrganisationCommunity(OrganisationId organisationId, UserId userId, Community community)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Community = community;
        }
    }
}