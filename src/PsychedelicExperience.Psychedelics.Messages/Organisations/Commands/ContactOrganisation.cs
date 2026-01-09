using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ContactOrganisation : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public string Email { get; }
        public string Subject { get; }
        public string Message { get; }

        public ContactOrganisation(OrganisationId organisationId, UserId userId, string email, string subject, string message)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Email = email;
            Subject = subject;
            Message = message;
        }
    }
}