using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationHealthcareProvider : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public HealthcareProvider HealthcareProvider { get; }

        public ChangeOrganisationHealthcareProvider(OrganisationId organisationId, UserId userId, HealthcareProvider healthcareProvider)
        {
            OrganisationId = organisationId;
            UserId = userId;
            HealthcareProvider = healthcareProvider;
        }
    }
}