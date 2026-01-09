using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class ChangeDocumentName : IRequest<Result>
    {
        public UserId UserId { get; }
        public DocumentId DocumentId { get; }
        public Name Name { get; }

        public ChangeDocumentName(UserId userId, DocumentId documentId, Name name)
        {
            UserId = userId;
            DocumentId = documentId;
            Name = name;
        }
    }
}