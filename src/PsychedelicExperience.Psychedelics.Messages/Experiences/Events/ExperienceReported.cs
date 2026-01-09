using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperienceReported : Event
    {
        public UserId UserId { get; set; }
        public ExperienceId ExperienceId { get; set; }
        public string Reason { get; set; }
    }
}