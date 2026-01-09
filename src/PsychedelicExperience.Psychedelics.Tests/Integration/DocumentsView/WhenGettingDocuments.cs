using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.DocumentsView
{
    public class WhenGettingDocuments : ServiceTestBase<IMediator>,
         IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly List<DocumentId> _ids = new List<DocumentId>();

        private DocumentsResult _result;

        public WhenGettingDocuments(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            base.Given(context);

            context.AddUser(_userId, Roles.ContentManager);

            _ids.Add(context.AddDocument(_userId, "What else 1"));
            _ids.Add(context.AddDocument(_userId, "What else 2"));
            _ids.Add(context.AddDocument(_userId, "What else 3"));
        }

        protected override void When(TestContext<IMediator> context)
        {
            var query = new GetDocuments(_userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenDocumentsShouldBeReturned()
        {
            _result.ShouldNotBeNull();
            _result.Documents.Length.ShouldBe(3);
            _result.Documents
                .Select(result => new DocumentId(result.DocumentId))
                .ShouldBeSubsetOf(_ids);
        }
    }
}
