using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationCenter : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public Center Center { get; }

        public ChangeOrganisationCenter(OrganisationId organisationId, UserId userId, Center center)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Center = center;
        }
    }

    public class ChangeOrganisationPractitioner : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public Practitioner Practitioner { get; }

        public ChangeOrganisationPractitioner(OrganisationId organisationId, UserId userId, Practitioner practitioner)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Practitioner = practitioner;
        }
    }

    public class ChangeOrganisationPerson : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public bool Person { get; }

        public ChangeOrganisationPerson(OrganisationId organisationId, UserId userId, bool person)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Person = person;
        }
    }
}