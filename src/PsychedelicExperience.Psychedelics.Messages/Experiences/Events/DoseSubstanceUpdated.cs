using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class DoseSubstanceUpdated : Event
    {
        public UserId UserId { get; set; }
        public DoseId DoseId { get; set; }
        public string PreviousSubstance { get; set; }
        public string Substance { get; set; }
        public ExperienceId ExperienceId { get; set; }
    }
}