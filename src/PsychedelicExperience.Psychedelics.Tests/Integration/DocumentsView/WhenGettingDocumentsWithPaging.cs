using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.DocumentsView
{
    public class WhenGettingDocumentsWithPaging : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly UserId _userId = UserId.New();

        private DocumentsResult _result;

        public WhenGettingDocumentsWithPaging(PsychedelicsIntegrationTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture)
        {
            _testOutputHelper = testOutputHelper;
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddAdministrator(_userId);

            for (var index = 0; index < 80; index++)
            {
                AddDocument(context, $"name{index:000}", "include" + (index % 2 == 1 ? "1" : ""));
            }
        }

        private DocumentId AddDocument(TestContext<IMediator> context, string title, string tag)
        {
            return context
                .BuildDocument(_userId, title, _testOutputHelper)
                .WithTag(tag)
                .Id;
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetDocuments(_userId, tags: new[] { "include" });
            _result = context.Service.ExecuteNowWithTimeout(query, _testOutputHelper);
        }

        [Fact]
        public void ThenOnePageShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Documents.Length.ShouldBe(20);
        }

        [Fact]
        public void ThenTheFirst20ItemsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            for (var index = 0; index < 20; index++)
            {
                _result.Documents[index].Name.ShouldBe($"name{index * 2:000}");
            }
        }

        [Fact]
        public void ThenTheTotalShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Total.ShouldBe(40);
        }
    }
}