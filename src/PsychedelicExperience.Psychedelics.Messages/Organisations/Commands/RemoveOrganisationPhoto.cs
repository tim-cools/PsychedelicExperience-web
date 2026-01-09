using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisationPhoto : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public PhotoId PhotoId { get; }
        public UserId UserId { get; }

        public RemoveOrganisationPhoto(OrganisationId organisationId, UserId userId, PhotoId photoId)
        {
            OrganisationId = organisationId;
            PhotoId = photoId;
            UserId = userId;
        }
    }
}