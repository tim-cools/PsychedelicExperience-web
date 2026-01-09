using System;
using System.Collections.Generic;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class ExperiencesApiClient
    {
        private const string BaseUrl = "api/experience/";

        private readonly ApiClient _apiClient;

        public ExperiencesApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Guid Create(TestAccount account, string title, List<Tuple<string, string>> data = null)
        {
            var request = ApiClient.AuthenticateRequest(account, BaseUrl);

            request.AddParameter("title", title);

            if (data != null)
            {
                foreach (var entry in data)
                {
                    request.AddParameter(entry.Item1, entry.Item2);
                }
            }

            var response = _apiClient.Post(request);

            var id = response.NewCreatedId();
            Get(account, id);
            return id;
        }

        public ExperiencesResult GetList(TestAccount account)
        {
            var request = ApiClient.AuthenticateRequest(account, BaseUrl);

            return _apiClient.Get<ExperiencesResult>(request);
        }

        public ExperienceDetails Get(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}");

            return _apiClient.Get<ExperienceDetails>(request);
        }
    }
}