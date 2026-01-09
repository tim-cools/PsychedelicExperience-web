using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ReportOrganisation : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public string Reason { get; }

        public ReportOrganisation(OrganisationId organisationId, UserId userId, string reason)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Reason = reason;
        }
    }
}