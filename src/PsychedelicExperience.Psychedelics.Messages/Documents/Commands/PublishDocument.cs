using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class PublishDocument : IRequest<Result>
    {
        public DocumentId DocumentId { get; }
        public UserId UserId { get; }

        public PublishDocument(DocumentId documentId, UserId userId)
        {
            DocumentId = documentId;
            UserId = userId;
        }
    }
}