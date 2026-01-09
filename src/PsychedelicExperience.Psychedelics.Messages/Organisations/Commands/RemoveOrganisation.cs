using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisation : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }

        public RemoveOrganisation(OrganisationId organisationId, UserId userId)
        {
            OrganisationId = organisationId;
            UserId = userId;
        }
    }

    public class RemoveOrganisations : IRequest<RemoveOrganisationsResult>
    {
        public UserId UserId { get; }
        public bool Confirm { get; }

        public RemoveOrganisations(UserId userId, bool confirm)
        {
            UserId = userId;
            Confirm = confirm;
        }
    }

    public class RemoveOrganisationsResult
    {
        public long CountWithOwner { get; set; }
        public long CountWithoutOwner { get; set; }
        public long Count { get; set; }
    }
}