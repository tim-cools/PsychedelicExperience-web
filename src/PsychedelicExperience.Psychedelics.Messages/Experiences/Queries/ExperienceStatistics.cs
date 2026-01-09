using System.Collections.Generic;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperienceStatistics
    {
        public int Public { get; set; }
        public int Restricted { get; set; }
        public int Private { get; set; }
        public int Total { get; set; }
        public IDictionary<string, int> Top5Substances { get; set; }
        public IDictionary<string, int> Top5Tags { get; set; }
    }
}