using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class GetDocumentImage : IRequest<PhotoDetails>
    {
        public DocumentId DocumentId { get; }

        public GetDocumentImage(DocumentId documentId)
        {
            DocumentId = documentId;
        }
    }
}