using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Web.Tests.Acceptance.Api.Client;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    internal static class ContextApiExtensions
    {
        internal static ApiClient GetApiClient(this ITestContext context)
        {
            return new ApiClient(context);
        }
    }
}