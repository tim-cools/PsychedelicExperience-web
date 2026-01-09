using System;
using System.Net;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class UserProfileApiClient
    {
        private const string BaseUrl = "api/profile/";

        private readonly ApiClient _apiClient;

        public byte[] Image => GetType().ReadResourceData("png-transparent.png");

        public UserProfileApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public void EnsureCreated(TestAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id);

            _apiClient.Get<UserProfileDetails>(request);
        }

        public UserProfileDetails Get(TestAccount requester, TestAccount requestedAccount = null)
        {
            if (requester == null) throw new ArgumentNullException(nameof(requester));

            var postFix = requestedAccount?.Id.ToString();
            var request = ApiClient.AuthenticateRequest(requester, BaseUrl + postFix);

            return _apiClient.Get<UserProfileDetails>(request);
        }

        public void ChangeEmail(TestAccount account, string email)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-email");
            request.AddParameter("email", email);

            _apiClient.Put(request);
        }

        public void ChangeAddress(TestAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-address");
            request.AddAddress();

            _apiClient.Put(request);
        }

        public void ChangeDisplayName(TestAccount account, string name)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-display-name");
            request.AddParameter("name", name);

            _apiClient.Put(request);
        }

        public void ChangeFullName(TestAccount account, string name)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-full-name");
            request.AddParameter("name", name);

            _apiClient.Put(request);
        }

        public void ChangeAvatar(TestAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-avatar");
            request.AddFile("files", Image, "test.jpg");

            _apiClient.Post(request, HttpStatusCode.OK);
        }

        public void ChangeTagLine(TestAccount account, string tagLine)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-tagline");
            request.AddParameter("tagLine", tagLine);

            _apiClient.Put(request);
        }

        public void ChangeDescription(TestAccount account, string description)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-description");
            request.AddParameter("description", description);

            _apiClient.Put(request);
        }

        public void ChangePrivacy(TestAccount account, PrivacyLevel level)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.Id == null) throw new ArgumentNullException(nameof(account.Id));

            var request = ApiClient.AuthenticateRequest(account, BaseUrl + account.Id + "/change-privacy");
            request.AddParameter("level", level);

            _apiClient.Put(request);
        }
    }
}