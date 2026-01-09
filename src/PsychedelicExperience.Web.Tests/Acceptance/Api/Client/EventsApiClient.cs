using System;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class EventsApiClient
    {
        private const string BaseUrl = "api/event/";

        private readonly ApiClient _apiClient;

        public EventsApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public EventsResult GetList(TestAccount account)
        {
            var request = ApiClient.AuthenticateRequest(account, BaseUrl);

            return _apiClient.Get<EventsResult>(request);
        }

        public EventDetails Get(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}");

            return _apiClient.Get<EventDetails>(request);
        }
    }
}