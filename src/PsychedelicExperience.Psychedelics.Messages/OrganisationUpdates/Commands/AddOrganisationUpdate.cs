using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands
{
    public class AddOrganisationUpdate : IRequest<Result>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationUpdateId UpdateId { get; }
        public OrganisationUpdatePrivacy Privacy { get; }
        public string Subject { get; }
        public string Content { get; }

        public AddOrganisationUpdate(UserId userId, OrganisationId id, OrganisationUpdateId updateId, string subject, string content, OrganisationUpdatePrivacy privacy) 
        {
            UserId = userId;
            OrganisationId = id;
            UpdateId = updateId;
            Privacy = privacy;
            Subject = subject;
            Content = content;
        }
    }
}