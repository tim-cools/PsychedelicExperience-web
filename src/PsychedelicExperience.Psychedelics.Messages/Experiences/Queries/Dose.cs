using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class Dose
    {
        public ShortGuid Id { get; set; }

        public int? TimeOffset { get; set; }

        public string Substance { get; set; }
        public string Form { get; set; }

        public decimal? Amount { get; set; }
        public string Unit { get; set; }
        public string Method { get; set; }

        public string Notes { get; set; }
    }
}