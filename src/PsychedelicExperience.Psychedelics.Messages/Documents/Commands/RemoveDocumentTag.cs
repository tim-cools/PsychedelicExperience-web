using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class RemoveDocumentTag : IRequest<Result>
    {
        public DocumentId DocumentId { get; }
        public UserId UserId { get; }
        public string TagName { get; }

        public RemoveDocumentTag(DocumentId documentId, UserId userId, string tagName)
        {
            DocumentId = documentId;
            UserId = userId;
            TagName = tagName;
        }
    }
}