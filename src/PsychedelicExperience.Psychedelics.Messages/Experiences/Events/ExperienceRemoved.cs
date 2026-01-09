using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperienceRemoved : Event
    {
        public UserId UserId { get; set; }
        public ExperienceId ExperienceId { get; set; }
    }
}