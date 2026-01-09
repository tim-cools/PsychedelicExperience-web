using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisationContact : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public string Type { get; set; }
        public string Value { get; }

        public RemoveOrganisationContact(OrganisationId organisationId, UserId userId, string type, string value)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Type = type;
            Value = value;
        }
    }
}