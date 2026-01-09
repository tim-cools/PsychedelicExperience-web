namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class ExperiencesSitemapResult
    {
        public string[] Tags { get; set; }
        public string[] Substances { get; set; }
        public ExperiencesSitemapEntry[] Experiences { get; set; }
    }
}