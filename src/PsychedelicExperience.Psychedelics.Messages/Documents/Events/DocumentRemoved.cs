using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Events
{
    public class DocumentRemoved : Event
    {
        public UserId UserId { get; set; }
        public DocumentId DocumentId { get; set; }
    }
}