using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands
{
    public class SetOrganisationUpdateContent : IRequest<Result>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationUpdateId UpdateId { get; }
        public string Content { get; }

        public SetOrganisationUpdateContent(UserId userId, OrganisationId id, OrganisationUpdateId updateId, string content)
        {
            UserId = userId;
            OrganisationId = id;
            UpdateId = updateId;
            Content = content;
        }
    }
}