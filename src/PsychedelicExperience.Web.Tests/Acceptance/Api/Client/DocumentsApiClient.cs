using System;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class DocumentsApiClient
    {
        private const string BaseUrl = "api/document/";

        private readonly ApiClient _apiClient;

        public DocumentsApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Guid Create(TestAccount account, Center center = null, Contact[] contacts = null, string website = null, string email = null, string phone = null)
        {
            var request = ApiClient.AuthenticateRequest(account, BaseUrl);
            var response = _apiClient.Post(request);

            var id = response.NewCreatedId();
            Get(account, id);
            return id;
        }

        public DocumentsResult GetList(TestAccount account)
        {
            var request = ApiClient.AuthenticateRequest(account, BaseUrl);

            return _apiClient.Get<DocumentsResult>(request);
        }

        public DocumentDetails Get(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}");

            return _apiClient.Get<DocumentDetails>(request);
        }
    }
}