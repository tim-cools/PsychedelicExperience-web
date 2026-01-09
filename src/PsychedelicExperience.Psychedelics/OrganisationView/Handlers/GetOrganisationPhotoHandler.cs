using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationPhotoHandler : QueryHandler<GetOrganisationPhoto, PhotoDetails>
    {
        public GetOrganisationPhotoHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<PhotoDetails> Execute(GetOrganisationPhoto query)
        {
            if (query.OrganisationId == null) throw new InvalidOperationException("OrganisationId is null");
            if (query.PhotoId == null) throw new BusinessException("PhotoId is null. Organisation: " + query.OrganisationId);

            var id = (Guid) query.OrganisationId;
            var photoId = query.PhotoId.Value;

            var organisation = await Session.LoadAsync<Organisation>(id);

            return organisation?.Photos?
                .FirstOrDefault(photo => photo != null && photo.PhotoId == photoId)
                .MapDetails();
        }
    }
}
