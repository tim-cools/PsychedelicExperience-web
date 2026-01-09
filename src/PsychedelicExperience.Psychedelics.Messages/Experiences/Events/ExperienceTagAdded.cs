using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperienceTagAdded : Event
    {
        public UserId UserId { get; set; }
        public ExperienceId ExperienceId { get; set; }
        public Name TagName { get; set; }
    }
}