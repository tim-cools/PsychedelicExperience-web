using System;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.Users;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class AccountApiClient
    {
        private const string BaseUrl = "api/account/";

        private readonly ApiClient _apiClient;
        private readonly IConfiguration _configuration;

        public AccountApiClient(ApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
        }

        public TestAccount CreateTestAccount()
        {
            var displayName = Guid.NewGuid().ToString("n");
            var password = "A." + Guid.NewGuid().ToString("n");
            var email = displayName + "@xxx.net";

            var id = CreateUser(displayName, email, password);

            return GetAccessToken(email, displayName, password, id);
        }

        public User GetAccount(TestAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl);

            return _apiClient.Get<User>(request);
        }

        public TestAccount GetAdministrator(TestContext<IMediator> context)
        {
            return GetAccessToken(
                context.Configuration.ApiTestAccountEMail(),
                "administrator",
                context.Configuration.ApiTestAccountPassword(), 
                null);
        }

        private Guid CreateUser(string displayName, string eMail, string password)
        {
            var request = new RestRequest("api/account/register");
            request.AddParameter("DisplayName", displayName);
            request.AddParameter("EMail", eMail);
            request.AddParameter("Password", password);
            request.AddParameter("ConfirmPassword", password);

            var response = _apiClient.Post(request);

            return response.NewCreatedId();
        }

        internal TestAccount GetAccessToken(string email, string displayName, string password, Guid? id)
        {
            var request = ApiClient.Request("token");

            request.AddParameter("grant_type", "password");
            request.AddParameter("username", email);
            request.AddParameter("password", password);
            request.AddParameter("client_id", _configuration.ApiClientId());
            request.AddParameter("client_secret", _configuration.ApiClientSecret());

            var response = _apiClient.Post(request, HttpStatusCode.OK);

            var content = JsonConvert.DeserializeObject<JObject>(response.Content);
            var accessToken = content.GetValue("access_token").Value<string>();
            
            return new TestAccount(email, displayName, accessToken, id);
        }

        public void AddRoleToUser(TestAccount requester, TestAccount accountToUpdate, Role role)
        {
            if (requester == null) throw new ArgumentNullException(nameof(requester));
            if (accountToUpdate == null) throw new ArgumentNullException(nameof(accountToUpdate));

            var request = ApiClient.AuthenticateRequest(requester, $"{BaseUrl}{accountToUpdate.Id}/roles/{role}");
            _apiClient.Post(request, HttpStatusCode.OK);
        }
    }
}