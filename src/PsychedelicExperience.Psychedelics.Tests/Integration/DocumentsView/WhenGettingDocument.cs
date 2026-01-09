using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using Shouldly;
using Xunit;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Membership.Users.Domain;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.DocumentsView
{
    public class WhenGettingDocument : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private UserId _userId;
        private DocumentDetails _result;
        private DocumentId _documentId;

        public WhenGettingDocument(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            _userId = context.AddUser(Roles.ContentManager);
            _documentId = context.AddDocument(_userId);
            context.AddDocument(_userId);
            context.AddDocument(_userId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetDocument(_userId, _documentId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenDocumentsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.DocumentId.ShouldBe((ShortGuid) _documentId.Value);
        }
    }
}
