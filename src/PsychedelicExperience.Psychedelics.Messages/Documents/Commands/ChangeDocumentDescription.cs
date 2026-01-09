using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class ChangeDocumentDescription : IRequest<Result>
    {
        public UserId UserId { get; }
        public DocumentId DocumentId { get; }
        public Description Description { get; }

        public ChangeDocumentDescription(UserId userId, DocumentId documentId, Description description)
        {
            UserId = userId;
            DocumentId = documentId;
            Description = description;
        }
    }
}