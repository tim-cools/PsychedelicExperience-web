using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationRelation
    {
        public ShortGuid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public string Relation { get; set; }
        public string OrganisationUrl { get; set; }
        public ShortGuid Photo { get; set; }
    }
}