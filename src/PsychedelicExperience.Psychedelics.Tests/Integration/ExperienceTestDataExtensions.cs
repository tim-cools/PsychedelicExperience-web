using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public static class ExperienceTestDataExtensions
    {
        public static ExperienceId AddExperience(this TestContext<IMediator> context, UserId userId, string title = "Found inner peace", string description = "in myself", ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = ExperienceId.New();

            var data = new ExperienceData();
            data.AddValue("title", title);
            data.AddValue("description", description);

            var command = new AddExperience(userId, id, data);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static TestContext<IMediator> AddExperienceTag(this TestContext<IMediator> context, UserId userId, ExperienceId experienceId, Name tagName, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new AddExperienceTag(experienceId, userId, tagName);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> SetExperiencePrivacy(this TestContext<IMediator> context, UserId userId, ExperienceId experienceId, PrivacyLevel privacy, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new SetExperiencePrivacy(experienceId, userId, privacy);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static DoseId AddDose(this TestContext<IMediator> context, UserId userId, ExperienceId experienceId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = DoseId.New();
            var command = new AddDose(experienceId, userId, id);
            context.ShouldSucceed(command);

            return id;
        }

        public static TestContext<IMediator> UpdateDoseSubstance(this TestContext<IMediator> context, UserId userId, DoseId doseId, string substance, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new UpdateDoseSubstance(userId, doseId, substance);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static ExperienceBuilder BuildExperience(this TestContext<IMediator> context, UserId userId, string title, string description, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var builder = new ExperienceBuilder(context, userId, testOutputHelper);
            return builder.BuildExperience(title, description);
        }
    }
}
