using System;
using PsychedelicExperience.Common.Aggregates;

namespace PsychedelicExperience.Psychedelics.Messages.Tags.Events
{
    public class TagAdded : Event
    {
        public Guid UserId { get; set; }
        public Guid TagId { get; set; }
        public String Name { get; set; }
    }
}