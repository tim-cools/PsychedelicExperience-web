using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperiencePrivateNotesChanged : Event
    {
        public UserId UserId { get; set; }
        public ExperienceId ExperienceId { get; set; }
        public EncryptedString Description { get; set; }
    }
}