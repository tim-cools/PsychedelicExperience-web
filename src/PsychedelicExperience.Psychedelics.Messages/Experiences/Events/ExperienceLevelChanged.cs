using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperienceLevelChanged : Event
    {
        public UserId UserId { get; set; }
        public ExperienceId ExperienceId { get; set; }
        public ExperienceLevel? PreviousLevel { get; set; }
        public ExperienceLevel? NewLevel { get; set; }
    }
}