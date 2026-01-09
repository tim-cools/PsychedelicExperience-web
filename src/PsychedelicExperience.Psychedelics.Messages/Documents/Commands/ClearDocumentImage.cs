using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class ClearDocumentImage : IRequest<Result>
    {
        public DocumentId DocumentId { get; }
        public UserId UserId { get; }

        public ClearDocumentImage(DocumentId documentId, UserId userId)
        {
            DocumentId = documentId;
            UserId = userId;
        }
    }
}