using System;
using System.Collections.Generic;

namespace PsychedelicExperience.Psychedelics.Messages.Documents.Queries
{
    public class DocumentsSitemapEntry
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<string> Tags { get; set; }

        public string Slug { get; set; }
    }
}