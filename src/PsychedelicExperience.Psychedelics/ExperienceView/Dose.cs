using System;

namespace PsychedelicExperience.Psychedelics.ExperienceView
{
    public class Dose
    {
        public Guid Id { get; set; }
        public int? TimeOffset { get; set; }

        public string Substance { get; set; }
        public string SubstanceNormalized { get; set; }

        public string Form { get; set; }

        public decimal? Amount { get; set; }

        public string Unit { get; set; }
        public string Method { get; set; }

        public string Notes { get; set; }
    }
}