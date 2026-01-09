using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.DocumentsView
{
    public class WhenGettingDocumentsByTags : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();
        private readonly List<ShortGuid> _included = new List<ShortGuid>();
        private readonly List<ShortGuid> _excluded = new List<ShortGuid>();

        private DocumentsResult _result;

        public WhenGettingDocumentsByTags(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            _included.Add(AddDocument(context, "include1", "include2"));

            _excluded.Add(AddDocument(context, "include1"));
            _excluded.Add(AddDocument(context, "include2"));
            _excluded.Add(AddDocument(context, "exclude1"));
            _excluded.Add(AddDocument(context, "exclude2"));
        }

        private Guid AddDocument(TestContext<IMediator> context, params string[] tags)
        {
            return (Guid)context
                .BuildDocument(_userId, "With " + tags.Length, _testOutputHelper)
                .WithTags(tags)
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetDocuments(_userId, tags: new[] { "include1", "include2" });
            _result = context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenDocumentsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Documents.Length.ShouldBe(1);

            _included.ForEach(value => _result.Documents.Select(result => result.DocumentId).ShouldContain(value));
        }

        [Fact]
        public void ThenTheOtherDocumentsShouldNotBeReturned()
        {
            _result.ShouldNotBeNull();

            _excluded.ForEach(value => _result.Documents.Select(result => result.DocumentId).ShouldNotContain(value));
        }
    }
}