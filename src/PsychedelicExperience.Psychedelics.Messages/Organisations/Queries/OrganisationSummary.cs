using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationSummary
    {
        public ShortGuid OrganisationId { get; set; }
        public string Url { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool Person { get; set; }
        public string[] Types { get; set; }
        
        public PhotoSummary[] Photos { get; set; }

        public int Followers { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int ReviewsCount { get; set; }
        public decimal? ReviewsRating{ get; set; }

        public Position Position { get; set; }
        public int? ReportsCount { get; set; }

        public CenterSummary Center { get; set; }
        public CommunitySummary Community { get; set; }
        public HealthcareProviderSummary HealthcareProvider { get; set; }
        public PractitionerSummary Practitioner { get; set; }
    }
}