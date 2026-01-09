using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class EventsSitemapEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<string> Tags { get; set; }

        public string Country { get; set; }
        public EventType EventType { get; set; }

        public string Slug() => Name.NormalizeForUrl();
    }
}