using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Commands
{
    public class ChangeDocumentImage : IRequest<Result>
    {
        public DocumentId DocumentId { get; }
        public UserId UserId { get; }
        public Image Image { get; set; }

        public ChangeDocumentImage(DocumentId documentId, UserId userId, Image image)
        {
            DocumentId = documentId;
            UserId = userId;
            Image = image;
        }
    }
}