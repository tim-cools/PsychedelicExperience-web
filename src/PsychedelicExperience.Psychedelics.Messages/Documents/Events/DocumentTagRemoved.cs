using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Events
{
    public class DocumentTagRemoved : Event
    {
        public DocumentId DocumentId { get; set; }
        public UserId UserId { get; set; }
        public string TagName { get; set; }
    }
}