using System;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class AvatarApiClient
    {
        private const string BaseUrl = "avatar/";

        private readonly ApiClient _apiClient;

        public AvatarApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public byte[] Get(TestAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id);

            return _apiClient.GetFile(request);
        }
    }
}