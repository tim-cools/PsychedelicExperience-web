using System;
using Marten.Events.Projections;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;

namespace PsychedelicExperience.Psychedelics.ExperienceStatisticsView
{
    public class ExperienceStatisticsProjection : ViewProjection<ExperienceStatistics, Guid>
    {
        public static readonly Guid StatisticsId = new Guid("500f6655-cc6d-4f09-b859-81d720afb9e5");

        public ExperienceStatisticsProjection()
        {
            ProjectEvent<DoseAdded>(@event => StatisticsId, Project);
            ProjectEvent<DoseRemoved>(@event => StatisticsId, Project);

            ProjectEvent<DoseSubstanceUpdated>(@event => StatisticsId, Project);

            ProjectEvent<ExperienceAdded>(@event => StatisticsId, Project);
            ProjectEvent<ExperienceRemoved>(@event => StatisticsId, Project);

            ProjectEvent<ExperiencePrivacyLevelChanged>(@event => StatisticsId, Project);

            ProjectEvent<ExperienceTagAdded>(@event => StatisticsId, Project);
            ProjectEvent<ExperienceTagRemoved>(@event => StatisticsId, Project);
        }

        private void Project(ExperienceStatistics statistics, DoseAdded @event)
        {
            statistics.Doses.Add();
        }

        private void Project(ExperienceStatistics statistics, DoseRemoved @event)
        {
            statistics.Doses.Remove();
        }

        private void Project(ExperienceStatistics statistics, DoseSubstanceUpdated @event)
        {
            statistics.Substance.Remove(@event.PreviousSubstance);
            statistics.Substance.Add(@event.Substance);
        }

        private void Project(ExperienceStatistics statistics, ExperienceAdded @event)
        {
            statistics.Total.Add();
            statistics.PrivacyCounter(PrivacyLevel.Private).Add();
        }

        private void Project(ExperienceStatistics statistics, ExperiencePrivacyLevelChanged @event)
        {
            statistics.PrivacyCounter(@event.PreviousLevel).Remove();
            statistics.PrivacyCounter(@event.NewLevel).Add();
        }

        private void Project(ExperienceStatistics statistics, ExperienceRemoved @event)
        {
            statistics.Total.Remove();
        }

        private void Project(ExperienceStatistics statistics, ExperienceTagAdded @event)
        {
            statistics.Tags.Add((string) @event.TagName);
        }

        private void Project(ExperienceStatistics statistics, ExperienceTagRemoved @event)
        {
            statistics.Tags.Remove((string)@event.TagName);
        }
    }
}