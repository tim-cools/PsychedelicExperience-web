using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationDetails
    {
        public ShortGuid OrganisationId { get; set; }

        public bool Person { get; set; }
        public string[] Types { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExternalDescription { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }

        public CenterDetails Center { get; set; }
        public CommunityDetails Community { get; set; }
        public HealthcareProviderDetails HealthcareProvider { get; set; }
        public PractitionerDetails Practitioner { get; set; }

        public OrganisationDetailsInfo Info { get; set; }
        public OrganisationDetailsInfo Warning { get; set; }

        public OrganisationAddress Address { get; set; }

        public ContactDetail[] Contacts { get; set; }
        public string[] Tags { get; set; }
        public PhotoSummary[] Photos { get; set; }

        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int ReviewsCount { get; set; }
        public decimal? ReviewsRating { get; set; }

        public OrganisationDetailsPrivileges Privileges { get; set; }

        public ReportDetails[] Reports { get; set; }
        public Owner[] Owners { get; set; }

        public OrganisationsReviewSummary[] Reviews { get; set; }
        public OrganisationRelation[] Relations { get; set; }
        public OrganisationRelation[] RelationsFrom { get; set; }
    }
}