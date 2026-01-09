using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationsReviewSummary
    {
        public ShortGuid ReviewId { get; set; }
        public DateTime? Visited { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public string Slug { get; set; }
    }
}