using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class DoseRemoved : Event
    {
        public UserId UserId { get; set; }
        public DoseId DoseId { get; set; }
        public ExperienceId ExperienceId { get; set; }
    }
}