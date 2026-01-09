using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class TopicInteractionClient
    {
        private const string BaseUrl = "api/interaction/";

        private readonly ApiClient _apiClient;

        public TopicInteractionClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public void Follow(TestAccount account, Guid topicId)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{topicId}/follow");
            _apiClient.Put(request);
        }

        public TopicInteraction Get(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}");

            return _apiClient.Get<TopicInteraction>(request);
        }
    }
}