using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class OrganisationReviewDetails
    {
        public ShortGuid OrganisationId { get; set; }
        public ShortGuid ReviewId { get; set; }

        public string UserName { get; set; }
        public ShortGuid UserId { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Visited { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ExternalDescription { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }

        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int Rating { get; set; }

        public OrganisationDetailsPrivileges Privileges { get; set; }

        public CenterReviewDetails Center { get; set; }
        public CommunityReviewDetails Community { get; set; }
        public HealthcareProviderReviewDetails HealthcareProvider { get; set; }
        public PractitionerReviewDetails Practitioner { get; set; }

        public ReviewExperienceSummary Experience { get; set; }
    }

    public class ReviewExperienceSummary
    {
        public ShortGuid ExperienceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}