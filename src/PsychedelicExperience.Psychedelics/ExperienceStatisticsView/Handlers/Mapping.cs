using PrivacyLevel = PsychedelicExperience.Psychedelics.Messages.Experiences.PrivacyLevel;

namespace PsychedelicExperience.Psychedelics.ExperienceStatisticsView.Handlers
{
    internal static class Mapping
    {
        internal static Messages.Experiences.Queries.ExperienceStatistics Map(this ExperienceStatistics statistics)
        {
            return new Messages.Experiences.Queries.ExperienceStatistics
            {
                Total = statistics.Total.Value,
                Public = statistics.PrivacyCounter(PrivacyLevel.Public).Value,
                Restricted = statistics.PrivacyCounter(PrivacyLevel.Restricted).Value,
                Private = statistics.PrivacyCounter(PrivacyLevel.Private).Value,
                Top5Substances = statistics.Substance.Top5(),
                Top5Tags = statistics.Tags.Top5(),
            };
        }
    }
}