using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public class OrganisationBuilder
    {
        private readonly TestContext<IMediator> _context;
        private readonly UserId _userId;
        private readonly ITestOutputHelper _testOutputHelper;

        public OrganisationId Id { get; private set; }

        public OrganisationBuilder(TestContext<IMediator> context, UserId userId, ITestOutputHelper testOutputHelper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userId = userId ?? throw new ArgumentNullException(nameof(userId));
            _testOutputHelper = testOutputHelper;
        }

        public OrganisationBuilder WithTag(string tag)
        {
            _context.AddOrganisationTag(_userId, Id, tag, _testOutputHelper);
            return this;
        }

        public OrganisationBuilder WithTags(string[] tags)
        {
            foreach (var tag in tags)
            {
                _context.AddOrganisationTag(_userId, Id, tag, _testOutputHelper);
            }
            return this;
        }

        public OrganisationBuilder BuildOrganisation(string name)
        {
            Id = _context.AddOrganisation(_userId, name, testOutputHelper: _testOutputHelper);

            return this;
        }
    }
}