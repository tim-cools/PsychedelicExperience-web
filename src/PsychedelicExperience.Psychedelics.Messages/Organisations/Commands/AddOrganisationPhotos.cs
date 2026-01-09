using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class AddOrganisationPhotos : IRequest<AddOrganisationPhotosResult>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public Photo[] Photos { get; set; }

        public AddOrganisationPhotos(OrganisationId organisationId, UserId userId, Photo[] photos)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Photos = photos;
        }
    }
}