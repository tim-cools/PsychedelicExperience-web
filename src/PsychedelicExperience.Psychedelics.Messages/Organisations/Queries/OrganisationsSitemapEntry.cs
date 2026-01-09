using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationsSitemapEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public IList<string> Tags { get; set; }
        public IList<string> Types { get; set; }

        public string Slug() => Name.NormalizeForUrl();
    }
}