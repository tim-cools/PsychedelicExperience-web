using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Events
{
    public class DocumentContentChanged : Event
    {
        public UserId UserId { get; set; }
        public DocumentId DocumentId { get; set; }
        public Description Content { get; set; }
    }
}