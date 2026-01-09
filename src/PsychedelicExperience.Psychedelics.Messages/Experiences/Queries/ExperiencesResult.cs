namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperiencesResult
    {
        public ExperienceSummary[] Experiences { get; set; }
        public long Page { get; set; }
        public long Total { get; set; }
        public long Last { get; set; }
    }
}