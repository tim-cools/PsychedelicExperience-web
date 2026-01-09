using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class GetDocument : IRequest<DocumentDetails>
    {
        public UserId UserId { get; }
        public string Slug { get; }
        public DocumentId DocumentId { get; }

        public GetDocument(UserId userId, DocumentId documentId)
        {
            UserId = userId;
            DocumentId = documentId;
        }

        public GetDocument(UserId userId, string slug)
        {
            UserId = userId;
            Slug = slug;
        }
    }
}
