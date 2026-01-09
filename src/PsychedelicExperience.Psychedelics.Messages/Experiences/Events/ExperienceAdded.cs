using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Events
{
    public class ExperienceAdded : Event
    {
        public ExperienceId ExperienceId { get; set; }
        public UserId UserId { get; set; }
        public DateTime? DateTime { get; set; }
        public Title Title { get; set; }
        public Description Description { get; set; }
        public Name Partner { get; set; }
    }
}
