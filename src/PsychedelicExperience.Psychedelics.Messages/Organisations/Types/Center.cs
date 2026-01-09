using System;
using System.Linq;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class Center
    {
        public CenterStatus Status { get; set; }
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

        public string NormalizeForSearch() =>
            NormalizeForSearch(
                Medicines?.Ingredients,
                Location?.Description,
                Location?.HowToGetThere,
                Environment?.MusicPlayed,
                Safety?.IntakeProcess?.Description,
                Safety?.IntegrationProcess?.Description,
                Safety?.MedicalFacilitiesNearby?.Description,
                Safety?.MedicalFacilitiesOnsite?.Description,
                Safety?.PsychologicalTherapisOnsite?.Description,
                Purpose?.PersonalDevelopment?.Description,
                Purpose?.ReligiousCeremonies?.Description,
                Purpose?.TreatmentOfAddictions?.Description,
                Purpose?.TreatmentOfPhysicalIllnesses?.Description,
                Purpose?.TreatmentOfPsychologicalDisorders?.Description,
                Team?.Description,
                Engagement?.ResearchProjects?.Description,
                Engagement?.SustainabilityProjects?.Description);

        private string NormalizeForSearch(params string[] normalizeForSearch)
        {
            return string.Join(' ', normalizeForSearch.Where(value => value != null).Select(value => value.NormalizeForSearch()));
        }
    }
}