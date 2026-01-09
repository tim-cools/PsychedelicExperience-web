using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Events
{
    public class DocumentAdded : Event
    {
        public UserId UserId { get; set; }
        public DocumentId DocumentId { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
