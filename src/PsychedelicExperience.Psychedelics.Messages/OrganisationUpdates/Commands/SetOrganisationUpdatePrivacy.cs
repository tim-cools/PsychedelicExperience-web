using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands
{
    public class SetOrganisationUpdatePrivacy : IRequest<Result>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationUpdateId UpdateId { get; }
        public OrganisationUpdatePrivacy Privacy { get; }

        public SetOrganisationUpdatePrivacy(UserId userId, OrganisationId id, OrganisationUpdateId updateId, OrganisationUpdatePrivacy privacy)
        {
            UserId = userId;
            OrganisationId = id;
            UpdateId = updateId;
            Privacy = privacy;
        }
    }
}