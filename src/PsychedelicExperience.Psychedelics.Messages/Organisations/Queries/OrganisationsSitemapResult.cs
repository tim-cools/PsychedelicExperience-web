using System;
using System.Collections.Generic;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationsSitemapResult
    {

        public string[] Types { get; set; }

        public OrganisationsTypeCountry[] TypesCountries { get; set; }
        public OrganisationsTypeTag[] TypesTags { get; set; }
        public OrganisationsTypeTagCountry[] TypesTagsCountries { get; set; }

        public OrganisationsSitemapEntry[] Organisations { get; set; }
    }
    
    public class OrganisationReviewsSitemapResult
    {
        public OrganisationReviewSitemapEntry[] Reviews { get; set; }
    }
    
    public class OrganisationReviewSitemapEntry
    {
        public Guid Id { get; set; }
        public Guid OrganisationId { get; set; }
        public string Name { get; set; }
        public string Slug() => Name.NormalizeForUrl();
    }
}