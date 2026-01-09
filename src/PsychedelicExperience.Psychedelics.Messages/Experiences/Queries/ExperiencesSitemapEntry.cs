using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperiencesSitemapEntry
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> Substances { get; set; }

        public string Slug() => Title.NormalizeForUrl();
    }
}