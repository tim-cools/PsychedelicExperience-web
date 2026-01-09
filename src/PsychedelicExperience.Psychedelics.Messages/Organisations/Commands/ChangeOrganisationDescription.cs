using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationDescription : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public string Description { get; }

        public ChangeOrganisationDescription(OrganisationId organisationId, UserId userId, string description)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Description = description;
        }
    }
}