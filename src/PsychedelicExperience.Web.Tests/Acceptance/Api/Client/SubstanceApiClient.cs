using System;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class SubstanceApiClient
    {
        private const string BaseUrl = "api/substance/";

        private readonly ApiClient _apiClient;

        public SubstanceApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public SubstanceResult Get(TestAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl);

            return _apiClient.Get<SubstanceResult>(request);
        }
    }
}