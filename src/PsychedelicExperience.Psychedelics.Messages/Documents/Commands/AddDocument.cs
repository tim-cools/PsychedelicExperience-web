using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class AddDocument : IRequest<Result>
    {
        public UserId UserId { get; }
        public DocumentId DocumentId { get; }
        public DocumentType DocumentType { get; }

        public AddDocument(UserId userId, DocumentId documentId, DocumentType documentType)
        {
            UserId = userId;
            DocumentId = documentId;
            DocumentType = documentType;
        }
    }
}
