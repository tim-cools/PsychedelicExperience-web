using System;
using System.Linq;
using System.Threading.Tasks;
using FastMember;
using Marten;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;

namespace PsychedelicExperience.Psychedelics.ExperienceView
{
    public class ExperienceProjection : EventProjection
    {
        private readonly ILogger<ExperienceProjection> _logger;

        public override Type ViewType => typeof(Experience);

        public ExperienceProjection(ILogger<ExperienceProjection> logger)
        {
            _logger = logger;

            ExperienceEventAsync<DoseAdded>(Project);
            ExperienceEventAsync<DoseRemoved>(Project);

            ExperienceEventAsync<DoseTimeOffsetUpdated>(Project);
            ExperienceEventAsync<DoseSubstanceUpdated>(Project);
            ExperienceEventAsync<DoseFormUpdated>(Project);
            ExperienceEventAsync<DoseAmountUpdated>(Project);
            ExperienceEventAsync<DoseUnitUpdated>(Project);
            ExperienceEventAsync<DoseMethodUpdated>(Project);
            ExperienceEventAsync<DoseNotesUpdated>(Project);

            ExperienceEventAsync<ExperienceAdded>(Project);
            EventAsync<ExperienceRemoved>(Project);

            ExperienceEventAsync<ExperienceTitleChanged>(Project);
            ExperienceEventAsync<ExperienceDateTimeChanged>(Project);
            ExperienceEventAsync<ExperienceLevelChanged>(Project);
            ExperienceEventAsync<ExperiencePrivacyLevelChanged>(Project);
            ExperienceEventAsync<ExperiencePrivateNotesChanged>(Project);
            ExperienceEventAsync<ExperiencePublicDescriptionChanged>(Project);
            ExperienceEventAsync<ExperienceSetChanged>(Project);
            ExperienceEventAsync<ExperienceSettingChanged>(Project);

            ExperienceEventAsync<ExperienceTagAdded>(Project);
            ExperienceEventAsync<ExperienceTagRemoved>(Project);
        }

        private void ExperienceEventAsync<T>(Action<Experience, T> handler) where T : Event
        {
            var typeAccessor = CreateTypeAccessor<T>();

            EventAsync<T>(async (session, id, @event) =>
            {
                var eventId = (Id) typeAccessor[@event, "ExperienceId"];
                if (eventId == null)
                {
                    _logger.LogError($"Could not get ExperienceId from event {@event.GetType()} : id: {id}");
                    return;
                }
                var experience = await GetView(session, eventId.Value, @event);

                handler(experience, @event);
            });
        }

        private static TypeAccessor CreateTypeAccessor<T>() where T : Event
        {
            var typeAccessor = TypeAccessor.Create(typeof(T));
            if (typeAccessor.GetMembers().FirstOrDefault(member => member.Name == "ExperienceId") == null)
            {
                throw new InvalidOperationException($"Event Type '{typeof(T).FullName}' has no property 'ExperienceId'");
            }
            return typeAccessor;
        }

        private void Project(Experience experience, DoseAdded @event)
        {
            var dose = new Dose { Id = (Guid) @event.DoseId };
            experience.Doses.Add(dose);
        }

        private void Project(Experience experience, DoseAmountUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.Amount = @event.Amount;
        }

        private void Project(Experience experience, DoseMethodUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.Method = @event.Method;
        }

        private void Project(Experience experience, DoseRemoved @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            experience.Doses.Remove(dose);
        }

        private void Project(Experience experience, DoseSubstanceUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.Substance = @event.Substance;
            dose.SubstanceNormalized = @event.Substance.NormalizeForSearch();
        }

        private void Project(Experience experience, DoseFormUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.Form = @event.Form;
        }

        private void Project(Experience experience, DoseNotesUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.Notes = @event.Notes;
        }

        private void Project(Experience experience, DoseTimeOffsetUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.TimeOffset = @event.TimeOffsetSeconds;
        }

        private void Project(Experience experience, DoseUnitUpdated @event)
        {
            var dose = experience.GetDose((Guid)@event.DoseId);
            dose.Unit = @event.Unit;
        }

        private void Project(Experience experience, ExperienceAdded @event)
        {
            experience.Created = @event.EventTimestamp;
            experience.UserId = (Guid) @event.UserId;
            experience.Title = (string) @event.Title;
            experience.PublicDescription= @event.Description?.Value;
            
            if (@event.Partner != null && !string.IsNullOrEmpty(@event.Partner.Value))
            {
                experience.Partners.Add(@event.Partner?.Value);
            }

            experience.DateTime = @event.DateTime;

            UpdateSearchField(experience);
        }

        private void Project(Experience experience, ExperienceLevelChanged @event)
        {
            experience.Level = @event.NewLevel.ToString();
        }

        private void Project(Experience experience, ExperiencePrivacyLevelChanged @event)
        {
            experience.PrivacyLevel = (PrivacyLevel) @event.NewLevel;
            UpdateSearchField(experience);
        }

        private void Project(Experience experience, ExperiencePrivateNotesChanged @event)
        {
            experience.PrivateDescription = @event.Description;
        }

        private void Project(Experience experience, ExperiencePublicDescriptionChanged @event)
        {
            experience.PublicDescription = (string)@event.Description;
            UpdateSearchField(experience);
        }

        private Task Project(IDocumentSession session, Guid id, ExperienceRemoved @event)
        {
            session.Delete<Experience>((Guid) @event.ExperienceId);

            return Task.FromResult(false);
        }

        private void Project(Experience experience, ExperienceSetChanged @event)
        {
            experience.SetDescription = (string) @event.SetDescription;
        }

        private void Project(Experience experience, ExperienceSettingChanged @event)
        {
            experience.SettingDescription = (string) @event.SettingDescription;

            UpdateSearchField(experience);
        }

        private void UpdateSearchField(Experience experience)
        {
            var searchString = $"{experience.Title.NormalizeForSearch()} " +
                               $"{experience.PublicDescription.NormalizeForSearch()} ";

            if (experience.PrivacyLevel == PrivacyLevel.Public)
            {
                searchString += $"{experience.SetDescription.NormalizeForSearch()} " +
                              $"{experience.SettingDescription.NormalizeForSearch()} ";
            }

            experience.SearchString = searchString;
        }

        private void Project(Experience experience, ExperienceTagAdded @event)
        {
            var name = (string)@event.TagName;
            experience.Tags.Add(name);
            experience.TagsNormalized.Add(name.NormalizeForSearch());
        }

        private void Project(Experience experience, ExperienceTagRemoved @event)
        {
            var name = (string)@event.TagName;
            experience.Tags.Remove(name);
            experience.Tags.Remove(name.NormalizeForSearch());
        }

        private void Project(Experience experience, ExperienceTitleChanged @event)
        {
            experience.Title = (string) @event.Title;
            UpdateSearchField(experience);
        }

        private void Project(Experience experience, ExperienceDateTimeChanged @event)
        {
            experience.DateTime = @event.DateTime;
        }

        private static async Task<Experience> GetView(IDocumentSession session, Guid id, Event @event)
        {
            var view = await session.LoadAsync<Experience>(id) ?? new Experience { Id = id };
            session.Store(view);
            view.LastUpdated = @event.EventTimestamp;
            return view;
        }
    }
}