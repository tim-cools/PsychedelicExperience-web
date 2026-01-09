using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class DoseNotesUpdated : Event
    {
        public UserId UserId { get; set; }
        public DoseId DoseId { get; set; }
        public string Notes { get; set; }
        public ExperienceId ExperienceId { get; set; }
    }
}