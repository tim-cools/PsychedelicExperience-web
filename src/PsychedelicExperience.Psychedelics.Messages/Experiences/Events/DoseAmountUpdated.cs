using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class DoseAmountUpdated : Event
    {
        public UserId UserId { get; set; }
        public DoseId DoseId { get; set; }
        public decimal? Amount { get; set; }
        public ExperienceId ExperienceId { get; set; }
    }
}