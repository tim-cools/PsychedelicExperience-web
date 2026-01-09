using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Web.Tests.Acceptance.Api.Client;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    public abstract class ApiTest : ServiceTestBase<IMediator>,
        IClassFixture<WebIntegrationTestFixture>
    {
        protected ApiClient ApiClient { get; private set; }

        protected ApiTest(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void PreGiven(TestContext<IMediator> context)
        {
            base.PreGiven(context);

            ApiClient = context.GetApiClient();
        }        
    }
}