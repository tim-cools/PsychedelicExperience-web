using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Surveys;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    //todo put in process manager if necessary
    public class AddExperienceHandler : CommandHandler<AddExperience, Result>
    {
        private ILogger<AddExperienceHandler> _logger;

        public AddExperienceHandler(IDocumentSession session, ILogger<AddExperienceHandler> logger) : base(session)
        {
            _logger = logger;
        }

        protected override async Task<Result> Execute(AddExperience command)
        {
            var user = await Session.LoadUserAsync(command.UserId);

            var experience = await StoreExperience(user, command);
            await StoreDose(user, experience, command.Data);
            await StoreDoses(user, experience, command.Data);
            await StoreSurvey(user, command.ExperienceId, command.Data);

            VerifyAllDataIsProcessed(command);

            return Result.Success;
        }

        private static void VerifyAllDataIsProcessed(AddExperience command)
        {
            if (!command.Data.Any()) return;

            var keys = string.Join(", ", command.Data.Select(key => key).ToArray());
            throw new InvalidOperationException("Unprocessed keys: " + keys);
        }

        private async Task StoreDose(User user, Experience experience, ExperienceData commandData)
        {
            var substance = commandData.PopString("substance");
            if (substance == null) return;

            await StoreDose(user, experience, substance);
        }

        private async Task StoreDose(User user, Experience experience, string substance)
        {
            var doseId = DoseId.New();
            var dose = await Session.LoadAggregate<Dose>(doseId);

            dose.Add(doseId, experience, user);
            dose.UpdateSubstance(user, substance);

            Session.StoreChanges(dose);
        }

        private async Task StoreDoses(User user, Experience experience, ExperienceData commandData)
        {
            var substances = commandData.PopArray("substances");
            if (substances == null) return;

            foreach (var substance in substances)
            {
                await StoreDose(user, experience, substance);
            }
        }

        private async Task<Experience> StoreExperience(User user, AddExperience command)
        {
            var date = Date(command);
            var title = Title(command);
            var description = Description(command);
            var partner = Partner(command);

            var experience = await Session.LoadAggregate<Experience>(command.ExperienceId);

            experience.Add(command.ExperienceId, user, date, title, description, partner);

            var privacyLevel = PrivacyLevel(command);
            if (privacyLevel != null)
            {
                experience.SetPrivacyLevel(user, privacyLevel.Value);
            }

            Session.StoreChanges(experience);

            return experience;
        }

        private DateTime? Date(AddExperience command)
        {
            var value = command.Data.PopString("date");
            try
            {
                return value != null ? DateTime.Parse(value) : (DateTime?) null;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error occured while parsing date: " + value);
                return null;
            }
        }

        private static Title Title(AddExperience command)
        {
            var value = command.Data.PopString("title");
            return value != null ? new Title(value) : null;
        }

        private static Description Description(AddExperience command)
        {
            var value = command.Data.PopString("description");
            return value != null ? new Description(value) : null;
        }

        private static Name Partner(AddExperience command)
        {
            var value = command.Data.PopString("partner");
            return value != null ? new Name(value) : null;
        }

        private static PrivacyLevel? PrivacyLevel(AddExperience command)
        {
            var value = command.Data.PopString("privacy");
            return value != null ? Enum.Parse<PrivacyLevel>(value) : (PrivacyLevel?) null;
        }
        private async Task StoreSurvey(User user, ExperienceId experienceId, ExperienceData commandData)
        {
            var surveyName = commandData.PopString("survey-id");
            if (surveyName == null) return;

            var surveyId = SurveyId.New();
            var survey = await Session.LoadAggregate<Survey>(surveyId);
            var surveyData = GetSurveyData(commandData);
            survey.Add(surveyId,  experienceId, user, surveyName, surveyData);

            Session.StoreChanges(survey);
        }

        private SurveyData GetSurveyData(ExperienceData commandData)
        {
            var data = new SurveyData();
            foreach (var key in commandData
                .Where(key => key.Contains("-"))
                .ToArray())
            {
                var value = commandData.PopRaw(key);
                data.Add(key, value);
            }

            return data;
        }
    }
}
