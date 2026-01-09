using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public class ExperienceBuilder
    {
        private readonly TestContext<IMediator> _context;
        private readonly UserId _userId;
        private readonly ITestOutputHelper _testOutputHelper;

        public ExperienceId Id { get; private set; }

        public ExperienceBuilder(TestContext<IMediator> context, UserId userId, ITestOutputHelper testOutputHelper)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            _context = context;
            _userId = userId;
            _testOutputHelper = testOutputHelper;
        }

        public ExperienceBuilder WithPrivacy(PrivacyLevel privacy)
        {
            _context.SetExperiencePrivacy(_userId, Id, privacy, _testOutputHelper);
            return this;
        }


        public ExperienceBuilder WithDose(string substance)
        {
            var doseId = _context.AddDose(_userId, Id);
            _context.UpdateDoseSubstance(_userId, doseId, substance, _testOutputHelper);
            return this;
        }

        public ExperienceBuilder WithTag(Name tag)
        {
            _context.AddExperienceTag(_userId, Id, tag, _testOutputHelper);
            return this;
        }

        public ExperienceBuilder WithTags(Name[] tags)
        {
            foreach (var tag in tags)
            {
                _context.AddExperienceTag(_userId, Id, tag, _testOutputHelper);
            }
            return this;
        }

        public ExperienceBuilder BuildExperience(string title, string description)
        {
            Id = _context.AddExperience(_userId, title, description, _testOutputHelper);

            return this;
        }
    }
}