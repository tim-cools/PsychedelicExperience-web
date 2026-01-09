using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperiencePublicDescriptionChanged : Event
    {
        public UserId UserId { get; set; }
        public ExperienceId ExperienceId { get; set; }
        public Description Description { get; set; }
    }
}