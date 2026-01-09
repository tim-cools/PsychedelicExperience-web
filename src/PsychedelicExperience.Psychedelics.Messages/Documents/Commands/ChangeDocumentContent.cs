using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class ChangeDocumentContent : IRequest<Result>
    {
        public UserId UserId { get; }
        public DocumentId DocumentId { get; }
        public Description Content { get; }

        public ChangeDocumentContent(UserId userId, DocumentId documentId, Description content)
        {
            UserId = userId;
            DocumentId = documentId;
            Content = content;
        }
    }
}