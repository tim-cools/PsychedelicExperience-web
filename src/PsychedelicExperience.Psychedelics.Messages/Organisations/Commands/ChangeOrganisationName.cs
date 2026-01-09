using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationName : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public Name Name { get; }

        public ChangeOrganisationName(OrganisationId organisationId, UserId userId, Name name)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Name = name;
        }
    }
}