using PsychedelicExperience.Web.Tests.Acceptance.Api.Client;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    public class ApiContext
    {
        public ApiClient ApiClient { get; }

        public ApiContext(ScenarioTestContext testContext)
        {
            ApiClient = testContext.GetApiClient();
        }
    }
}