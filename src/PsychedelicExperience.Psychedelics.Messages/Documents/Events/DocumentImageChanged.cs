using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Events
{
    public class DocumentImageChanged : Event
    {
        public DocumentId DocumentId { get; set; }
        public UserId UserId { get; set; }
        public Image Image { get; set; }
    }
}