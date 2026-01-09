using PsychedelicExperience.Psychedelics.Messages.Experiences;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class CenterReview
    {
        public int NumberOfPeople { get; set; }
        public int? NumberOfFacilitators { get; set; }
        public Rating Location { get; set; }
        public Rating Accommodation { get; set; }
        public Rating Facilitators { get; set; }
        public Rating Medicine { get; set; }

        public ExperienceId ExperienceId { get; set; }
        public string Facilitator { get; set; }

        public Rating Honest { get; set; }
        public Rating Secure { get; set; }
        public Rating Preparation { get; set; }
        public Rating BookingProcess { get; set; }
        public Rating FollowUp { get; set; }
    }
}