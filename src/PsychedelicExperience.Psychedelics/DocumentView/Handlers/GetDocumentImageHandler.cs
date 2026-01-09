using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;

namespace PsychedelicExperience.Psychedelics.DocumentView.Handlers
{
    public class GetDocumentImageHandler : QueryHandler<GetDocumentImage, PhotoDetails>
    {
        public GetDocumentImageHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<PhotoDetails> Execute(GetDocumentImage query)
        {
            var document = await Session.LoadAsync<Document>(query.DocumentId);

            return document?.Image != null 
                ? new PhotoDetails { Id = document.Image.PhotoId, FileName = document.Image.FileName } 
                : null;
        }
    }
}
