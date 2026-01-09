using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Marten.Util;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using RestSharp.Portable;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class OrganisationsApiClient
    {
        private const string BaseUrl = "api/organisation/";

        private readonly ApiClient _apiClient;

        public OrganisationsApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public Guid Create(TestAccount account, string name = null, Center center = null, Contact[] contacts = null, string website = null, string email = null, string phone = null, bool person = false)
        {
            var request = ApiClient.AuthenticateRequest(account, BaseUrl);
            request.AddParameter("name", name ?? Guid.NewGuid().ToString());
            request.AddParameter("person", person);
            request.AddParameter("description", Guid.NewGuid().ToString());

            request.AddParameter("tags[]", "tag1");

            if (contacts != null)
            {
                for (var index = 0; index < contacts.Length; index++)
                {
                    var contact = contacts[index];
                    request.AddParameter($"contacts[{index}][type]", contact.Type);
                    request.AddParameter($"contacts[{index}][value]", contact.Value);
                }
            }
            request.AddParameter("website", website);
            request.AddParameter("eMail", email);
            request.AddParameter("phone", phone);

            request.Add(center);          
            request.AddAddress();

            var response = _apiClient.Post(request);

            var id = response.NewCreatedId();
            Get(account, id);
            return id;
        }

        public Guid CreateReview(TestAccount account, ShortGuid organisationId, DateTime? visited, CenterReview center = null)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/review");
            request.AddParameter("name", Guid.NewGuid().ToString());

            request.AddParameter("visited", (visited ?? DateTime.Now).ToString("O"));

            request.AddParameter("rating.description", Guid.NewGuid().ToString());
            request.AddParameter("rating.value", 2);

            request.AddParameter("location.description", Guid.NewGuid().ToString());
            request.AddParameter("location.value", 2);

            request.AddParameter("accommodation.description", Guid.NewGuid().ToString());
            request.AddParameter("accommodation.value", 2);

            request.AddParameter("facilitators.description", Guid.NewGuid().ToString());
            request.AddParameter("facilitators.value", 2);

            request.AddParameter("medicine.description", Guid.NewGuid().ToString());
            request.AddParameter("medicine.value", 2);

            request.Add(center);

            var response = _apiClient.Post(request);

            var id = response.NewCreatedId();
            GetReview(account, organisationId, id);
            return id;
        }

        public Guid CreateReviewWithExperience(TestAccount account, ShortGuid organisationId, DateTime? visited, CenterReview center = null)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/review");
            request.AddParameter("name", Guid.NewGuid().ToString());

            request.AddParameter("visited", (visited ?? DateTime.Now).ToString("O"));

            request.AddParameter("rating.description", Guid.NewGuid().ToString());
            request.AddParameter("rating.value", 2);

            request.AddParameter("location.description", Guid.NewGuid().ToString());
            request.AddParameter("location.value", 2);

            request.AddParameter("accommodation.description", Guid.NewGuid().ToString());
            request.AddParameter("accommodation.value", 2);

            request.AddParameter("facilitators.description", Guid.NewGuid().ToString());
            request.AddParameter("facilitators.value", 2);

            request.AddParameter("medicine.description", Guid.NewGuid().ToString());
            request.AddParameter("medicine.value", 2);

            request.AddParameter("experience.title", "new experience");
            request.AddParameter("experience.description", "new description");

            request.Add(center);

            var response = _apiClient.Post(request);

            var id = response.NewCreatedId();
            GetReview(account, organisationId, id);
            return id;
        }

        public Guid CreateEvent(TestAccount account, string name, ShortGuid organisationId)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/event");
            request.AddParameter("name", name);
            request.AddParameter("description", "This is a very long description. This is a very long description. This is a very long description. This is a very long description. This is a very long description.");
            request.AddParameter("startDateTime", DateTime.Now.AddHours(1).ToString("O"));
            request.AddParameter("endDateTime", DateTime.Now.AddDays(1).ToString("O"));
            request.AddParameter("eventType", "Community");
            request.AddParameter("privacy", "Public");
            request.AddParameter("locationName", "Somewhere");
            request.AddAddress();

            var response = _apiClient.Post(request);

            var id = response.NewCreatedId();
            _apiClient.Events.Get(account, id);
            return id;
        }

        public OrganisationsResult GetList(TestAccount account, string query = null, string[] types = null, Format format = Format.Json)
        {
            var queryString = QueryString(query, types, format);

            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{queryString}");

            return format == Format.Json
                ? (OrganisationsResult) _apiClient.Get<JsonOrganisationsResult>(request)
                : new CsvOrganisationsResult {Bytes = _apiClient.GetFile(request)};
        }

        private static string QueryString(string query, string[] types, Format format)
        {
            var values = new NameValueCollection { };
            if (query != null)
            {
                values.Add("query", query);
            }

            if (types != null)
            {
                foreach (var type in types)
                {
                    values.Add("types", type);
                }
            }
            values.Add("format", format.ToString());

            var queryString = Common.Web.QueryString.Build(values);
            return queryString;
        }

        public OrganisationDetails Get(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}");

            return _apiClient.Get<OrganisationDetails>(request);
        }

        public OrganisationReviewResult GetReview(TestAccount account, ShortGuid id, ShortGuid reviewId)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/review/{reviewId}");

            return _apiClient.Get<OrganisationReviewResult>(request);
        }

        public void SetWarning(TestAccount account, Guid id, string title, string content)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/warning");
            request.AddParameter("title", title);
            request.AddParameter("content", content);

            _apiClient.Put(request);
        }

        public void SetInfo(TestAccount account, Guid id, string title, string content)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/info");
            request.AddParameter("title", title);
            request.AddParameter("content", content);

            _apiClient.Put(request);
        }

        public void DeleteWarning(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/warning");
            _apiClient.Delete(request);
        }

        public void DeleteInfo(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/info");
            _apiClient.Delete(request);
        }

        public void UpdateContact(TestAccount account, Guid id, string type, string value)
        {
            value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/contact/{type}/{value}");
            _apiClient.Post(request, HttpStatusCode.OK);
        }

        public void DeleteContact(TestAccount account, Guid id, string type, string value)
        {
            value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/contact/{type}/{value}");
            _apiClient.Delete(request);
        }

        public void SetOwner(TestAccount administrator, Guid id, string email)
        {
            var request = ApiClient.AuthenticateRequest(administrator, $"{BaseUrl}{id}/owners/{email}");
            _apiClient.Post(request);
        }

        public void RemoveOwner(TestAccount administrator, Guid id, TestAccount account)
        {
            var request = ApiClient.AuthenticateRequest(administrator, $"{BaseUrl}{id}/owners/{account.Id}");
            _apiClient.Delete(request);
        }

        public void SetCenter(TestAccount account, Guid id, Center center)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/center");
            request.AddParameter("location.description", center.Location.Description);

            _apiClient.Put(request);
        }

        public void SetPractitioner(TestAccount account, Guid id, Practitioner practitioner)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/practitioner");
            var jsonString = JsonConvert.SerializeObject(practitioner);

            try
            {
                AddProperties(request, practitioner);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            _apiClient.Put(request);
        }

        private void AddProperties(RestRequest request, object value, string root = null)
        {
            if (value == null) return;

            var properties = value.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propValue = property.GetValue(value);
                var name = root != null ? $"{root}.{property.Name.ToCamelCase()}" : property.Name.ToCamelCase() ;
                if (propValue == null)
                {
                }
                else if (property.PropertyType.IsArray)
                {
                    var array = propValue as Array;
                    for (var i = 0; i < array.Length; i++)
                    {
                        var arrayValue = array.GetValue(i);
                        request.AddParameter($"{name}[{i}]", arrayValue);
                    }
                }
                else if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    request.AddParameter(name, propValue);
                }
                else
                {
                    AddProperties(request, propValue, name);
                }
            }
        }

        public void DeleteCenter(TestAccount account, Guid id)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{id}/center");

            _apiClient.Delete(request);
        }

        public void Link(TestAccount account, Guid organisationId, Guid organisationTargetId, string relation)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/link/{organisationTargetId}/{relation}");
            _apiClient.Put(request);
        }

        public void Unlink(TestAccount account, Guid organisationId, Guid organisationTargetId, string relation)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/link/{organisationTargetId}/{relation}");
            _apiClient.Delete(request);
        }

        public void AddType(TestAccount account, Guid organisationId, string name)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/types/{name}");
            _apiClient.Post(request, HttpStatusCode.OK);
        }

        public void RemoveType(TestAccount account, Guid organisationId, string name)
        {
            var request = ApiClient.AuthenticateRequest(account, $"{BaseUrl}{organisationId}/types/{name}");
            _apiClient.Delete(request);
        }

        public Tag[] GetTypes()
        {
            var request = ApiClient.Request($"{BaseUrl}types");
            return _apiClient.Get<Tag[]>(request);
        }
    }
}