using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Events
{
    public class DocumentSlugChanged : Event
    {
        public UserId UserId { get; set; }
        public DocumentId DocumentId { get; set; }
        public Name Slug { get; set; }        
    }
}