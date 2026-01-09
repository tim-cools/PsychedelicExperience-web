using System;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class CenterDetails
    {
        public string Status { get; set; }
        public DateTime? OpenSince { get; set; }

        public Medicines Medicines { get; set; }
        public LocationDetails Location { get; set; }
        public Accommodation Accommodation { get; set; }
        public EnvironmentDetails Environment { get; set; }
        public Safety Safety { get; set; }
        public Purpose Purpose { get; set; }
        public Team Team { get; set; }
        public Engagement Engagement { get; set; }
        public GroupSize GroupSize { get; set; }
        public Facilitators Facilitators { get; set; }
    }
}