using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class ChangeDocumentSlug : IRequest<Result>
    {
        public UserId UserId { get; }
        public DocumentId DocumentId { get; }
        public Name Slug { get; }

        public ChangeDocumentSlug(UserId userId, DocumentId documentId, Name slug)
        {
            UserId = userId;
            DocumentId = documentId;
            Slug = slug;
        }
    }
}