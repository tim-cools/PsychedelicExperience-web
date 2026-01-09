using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands
{
    public class SetOrganisationUpdateSubject : IRequest<Result>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationUpdateId UpdateId { get; }
        public string Subject { get; }

        public SetOrganisationUpdateSubject(UserId userId, OrganisationId id, OrganisationUpdateId updateId, string subject)
        {
            UserId = userId;
            OrganisationId = id;
            UpdateId = updateId;
            Subject = subject;
        }
    }
}