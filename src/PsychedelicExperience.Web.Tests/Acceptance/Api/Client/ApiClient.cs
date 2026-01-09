using System;
using System.Net;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Tests;
using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using Method = RestSharp.Portable.Method;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Client
{
    public class ApiClient
    {
        private readonly ITestContext _context;

        public UserProfileApiClient UserProfile { get; }
        public AccountApiClient Account { get; }
        public OrganisationsApiClient Organisations { get; }
        public EventsApiClient Events { get; }
        public DocumentsApiClient Documents { get; }
        public ExperiencesApiClient Experiences { get; }
        public AvatarApiClient Avatar { get; }
        public SubstanceApiClient Substances { get; }
        public TopicInteractionClient TopicInteraction { get; }

        internal ApiClient(ITestContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Organisations = new OrganisationsApiClient(this);
            Events = new EventsApiClient(this);
            Documents = new DocumentsApiClient(this);
            Experiences = new ExperiencesApiClient(this);
            Account = new AccountApiClient(this, _context.Configuration);
            UserProfile = new UserProfileApiClient(this);
            Avatar = new AvatarApiClient(this);
            Substances = new SubstanceApiClient(this);
            TopicInteraction = new TopicInteractionClient(this);
        }

        private RestClient CreateClient()
        {
            var restClient = new RestClient(_context.Configuration.ApiHostName());
            restClient.IgnoreResponseStatusCode = true;
            return restClient;
        }

        internal T Get<T>(RestRequest request)
        {
            Console.WriteLine($"GET {typeof(T).FullName}: {request.Resource}");

            var client = CreateClient();

            var start = DateTime.Now;
            IRestResponse response;
            do
            {
                request.Method = Method.GET;
                response = client.Execute(request).GetResult();
            } while (response.StatusCode == HttpStatusCode.Accepted
                     && (DateTime.Now - start).TotalSeconds < 30);

            ShouldBe(HttpStatusCode.OK, response);

            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (JsonReaderException exception)
            {
                throw  new InvalidOperationException("Could not parse json: " + response.Content, exception);
            }
        }

        public byte[] GetFile(RestRequest request)
        {
            Console.WriteLine($"GET File: {request.Resource}");

            var client = CreateClient();

            request.Method = Method.GET;
            var response = client.Execute(request).GetResult();

            ShouldBe(HttpStatusCode.OK, response);

            return response.RawBytes;
        }

        internal IRestResponse Post(RestRequest request, HttpStatusCode code = HttpStatusCode.Created)
        {
            var client = CreateClient();

            Console.WriteLine($"POST: {CreateClient().BaseUrl}{request.Resource}");

            request.Method = Method.POST;
            var response = client.Execute(request).GetResult();

            return ShouldBe(code, response);
        }

        internal void Put(RestRequest request)
        {
            Put<dynamic>(request);
        }

        internal void Delete(RestRequest request)
        {
            var client = CreateClient();

            Console.WriteLine($"DELETE: {CreateClient().BaseUrl}{request.Resource}");

            request.Method = Method.DELETE;
            
            var response = client.Execute(request).GetResult();
            ShouldBe(HttpStatusCode.OK, response);
        }

        internal T Put<T>(RestRequest request)
        {
            Console.WriteLine($"PUT: {request.Resource}");

            var client = CreateClient();

            request.Method = Method.PUT;
            var response = client.Execute(request).GetResult();

            ShouldBe(HttpStatusCode.OK, response);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private static IRestResponse ShouldBe(HttpStatusCode code, IRestResponse response)
        {
            //if (response.ErrorException != null)
            //{
            //    var message = $"{response.ErrorMessage}";
            //    throw new InvalidOperationException(message, response.ErrorException);
            //}

            if (response.StatusCode != code)
            {
                var message = $"{response.Request.Resource}: {response.StatusCode}: {response.Content}";
                throw new InvalidOperationException(message);
            }
            return response;
        }

        internal static RestRequest Request(string resource)
        {
            var request = new RestRequest(resource);
            Console.WriteLine($"Request: {resource}");
            return request;
        }

        internal static RestRequest AuthenticateRequest(TestAccount account, string resource)
        {
            var request = new RestRequest(resource);
            request.AddHeader("Authorization", "Bearer " + account.AccessToken);
            Console.WriteLine($"AuthenticateRequest: {resource}");
            return request;
        }
    }
}