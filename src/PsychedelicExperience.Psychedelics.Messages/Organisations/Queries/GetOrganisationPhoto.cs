using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisationPhoto : IRequest<PhotoDetails>
    {
        public OrganisationId OrganisationId { get; }
        public PhotoId PhotoId { get; set; }

        public GetOrganisationPhoto(OrganisationId organisationId, PhotoId photoId)
        {
            OrganisationId = organisationId;
            PhotoId = photoId;
        }
    }
}