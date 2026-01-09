using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class CenterReviewDetails
    {
        public int NumberOfPeople { get; set; }
        public int? NumberOfFacilitators { get; set; }
        
        public RatingDetails Location { get; set; }
        public RatingDetails Accommodation { get; set; }
        public RatingDetails Facilitators { get; set; }
        public RatingDetails Medicine { get; set; }

        public ShortGuid ExperienceId { get; set; }
        public string Facilitator { get; set; }

        public RatingDetails Honest { get; set; }
        public RatingDetails Secure { get; set; }
        public RatingDetails Preparation { get; set; }
        public RatingDetails BookingProcess { get; set; }
        public RatingDetails FollowUp { get; set; }
    }

    public class PractitionerReviewDetails
    {
        public string Why { get; set; }
        public string What { get; set; }
        public string PractitionerContribution { get; set; }
        public string Value { get; set; }

        public WorkPrice Price { get; set; }

        public RatingDetails PersonalContact { get; set; }
        public RatingDetails Professionalism { get; set; }
        public RatingDetails Organization { get; set; }
        public RatingDetails ProfileMatchesExperience { get; set; }
    }
}