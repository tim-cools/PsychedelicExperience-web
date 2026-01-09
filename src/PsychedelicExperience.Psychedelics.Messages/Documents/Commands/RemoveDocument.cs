using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class RemoveDocument : IRequest<Result>
    {
        public UserId UserId { get; }
        public DocumentId DocumentId { get; }

        public RemoveDocument(UserId userId, DocumentId documentId)
        {
            UserId = userId;
            DocumentId = documentId;
        }
    }
}