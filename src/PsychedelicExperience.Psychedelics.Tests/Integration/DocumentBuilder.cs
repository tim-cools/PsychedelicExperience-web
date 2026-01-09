using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public class DocumentBuilder
    {
        private readonly TestContext<IMediator> _context;
        private readonly UserId _userId;
        private readonly ITestOutputHelper _testOutputHelper;

        public DocumentId Id { get; private set; }

        public DocumentBuilder(TestContext<IMediator> context, UserId userId, ITestOutputHelper testOutputHelper)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (userId == null) throw new ArgumentNullException(nameof(userId));

            _context = context;
            _userId = userId;
            _testOutputHelper = testOutputHelper;
        }

        public DocumentBuilder WithTag(string tag)
        {
            _context.AddDocumentTag(Id, _userId, tag, _testOutputHelper);
            return this;
        }

        public DocumentBuilder WithTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _context.AddDocumentTag(Id, _userId, tag, _testOutputHelper);
            }
            return this;
        }

        public DocumentBuilder BuildDocument(string name)
        {
            Id = _context.AddDocument(_userId, _testOutputHelper);
            _context.SetDocumentName(Id, _userId, name);

            return this;
        }
    }
}