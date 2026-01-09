using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands
{
    public class RemoveOrganisationUpdate : IRequest<Result>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationUpdateId UpdateId { get; }

        public RemoveOrganisationUpdate(UserId userId, OrganisationId id, OrganisationUpdateId updateId)
        {
            UserId = userId;
            OrganisationId = id;
            UpdateId = updateId;
        }
    }
}