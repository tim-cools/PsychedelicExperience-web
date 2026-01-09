using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using Marten;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Hubspot.Commands;
using PsychedelicExperience.Psychedelics.OrganisationView;

namespace PsychedelicExperience.Psychedelics.Hubspot
{
    public class GetHubspotDefaultsHandler : QueryHandler<GetHubspotDefaults, HubspotDefaultsResult>
    {
        private readonly IConfiguration _configuration;

        public GetHubspotDefaultsHandler(IQuerySession session, IConfiguration configuration) : base(session)
        {
            _configuration = configuration;
        }

        /*
        public T GetById<T>(long companyId, HubSpotApi api) where T : CompanyHubSpotModel, new()
        {
            string absoluteUriPath = string.Format("{0}/companies/{1}", (object) new T().RouteBasePath, (object) companyId);
            try
            {
                return api._client.Execute<T>(absoluteUriPath, Method.GET, true);
            }
            catch (HubSpotException ex)
            {
                if (ex.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return default (T);
                throw;
            }
        }
*/
        protected override async Task<HubspotDefaultsResult> Execute(GetHubspotDefaults query)
        {
            var api = new HubSpotApi(_configuration.HubspotApi());
            var company = api.Company.GetById<Company>(query.HubspotId);

            return new HubspotDefaultsResult
            {
                Company = new CompanyData
                {
                    Name = company.Name,
                    Description = company.Description,
                    Email = company.Email,
                    Website = company.Website,
                    LinkedIn = company.LinkedIn,
                    Facebook = company.Facebook,
                    Instagram = company.Instagram,
                    Twitter = company.Twitter,
                    YouTube = company.YouTube,
                    Address = $"{company.Address}, {company.City}, {company.PostalCode}, {company.Country}",
                    Phone  = company.Phone
                },
                Existing = FindOrganisations(company)
            };
        }

        private OrganisationData[] FindOrganisations(Company company)
        {
            var organisations = Session.Query<OrganisationView.Organisation>()
                .Where(organisation => organisation.Name.Contains(company.Name)
                                       || organisation.Contacts.Any(contact => contact.Value == company.Website))
                .ToArray();

            return organisations.Select(organisation => new OrganisationData
            {
                Name = organisation.Name,
                Url = organisation.GetUrl()
            }).ToArray();
        }
    }

    public class Company : CompanyHubSpotModel
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "website")]
        public string Website { get; set; }

        [DataMember(Name = "linkedin_company_page")]
        public string LinkedIn { get; set; }

        [DataMember(Name = "facebook_company_page")]
        public string Facebook { get; set; }

        [DataMember(Name = "instagram")]
        public string Instagram { get; set; }

        [DataMember(Name = "twitter")]
        public string Twitter { get; set; }

        [DataMember(Name = "youtube")]
        public string YouTube { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "zip")]
        public string PostalCode { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        //public bool Exists { get; set; }
    }

    public class CompareHandler : CommandHandler<Compare, CompareResult>
    {
        private readonly IConfiguration _configuration;

        public CompareHandler(IDocumentSession session, IConfiguration configuration)
            : base(session)
        {
            _configuration = configuration;
        }

        protected override async Task<CompareResult> Execute(Compare command)
        {
            var currentUser = await Session.LoadUserAsync(command.CurrentUserId);
            if (currentUser == null)
            {
                throw new InvalidOperationException($"User not found: {command.CurrentUserId}");
            }

            var api = new HubSpotApi(_configuration.HubspotApi());

            var result = new CompareResult();
            var pexOrganisations = await Organisatoins();
            var companies = Companies(api);
            var contacts = Contacts(api);


            /*
            foreach (var pexOrganisation in pexOrganisations)
            {
                var domain = Domain(pexOrganisation, result);
                if (domain == null) continue;

                var company = ByComain(companies, pexOrganisation, domain, result);
                if (company == null) continue;

                var contact = OrganisationContacts(company, contacts, result);
                if (contact == null) continue;

                CompareW(pexOrganisation, company, contact, result);

                companies.Remove(company);
            }
            */

            var companiesByDomain = pexOrganisations
                .Select(org =>
                new {
                    domain = Domain(org, new CompareResult()),
                    org
                })
                .Where(org => org.domain != null)
                .ToDictionary(org => org.domain, org => org.org);

            foreach (var company in companies)
            {
                var domain = Domain(company, result);
                if (domain == null) continue;

                var exists = companiesByDomain.TryGetValue(domain, out _);

                SetExixtsField(company, api, exists);
            }

            return result;
        }

        private void SetExixtsField(Company company, HubSpotApi api, bool exists)
        {
            //company.Exists = exists;

            api.Company.Update(company);
        }

        private CompanyHubSpotModel ByComain(List<CompanyHubSpotModel> companies, CompanyHubSpotModel pexOrganisation, string domain, CompareResult result)
        {
            throw new NotImplementedException();
        }

        private void CompareW(Organisation pexOrganisation, CompanyHubSpotModel company, ContactHubSpotModel contact, CompareResult result)
        {
            var diff = new StringBuilder();

            if (pexOrganisation.Name != company.Name)
            {
                diff.AppendLine($"Company name: {pexOrganisation.Name} - {company.Name}");
            }

            if (contact.Email == null || contact.Email != EMail(pexOrganisation) )
            {
                diff.AppendLine($"Company email: {pexOrganisation.Name} - {company.Name}");
            }
            if (diff.Length > 0)
            {
                result.Log("Different", pexOrganisation.Id, pexOrganisation.Name, diff.ToString());
            }
        }

        private string EMail(Organisation pexOrganisation)
        {
            return pexOrganisation.Contacts
                .FirstOrDefault(contact => contact.Type == ContactTypes.EMail)
                ?.Value;
        }

        private ContactHubSpotModel OrganisationContacts(CompanyHubSpotModel company, List<ContactHubSpotModel> contacts, CompareResult result)
        {
            var contact = contacts.FirstOrDefault(contact => contact.AssociatedCompanyId == company.Id);
            if (contact == null)
            {
                result.Log("HasNoHubspotContact", null, company.Domain);
            }

            return contact;
        }

        private CompanyHubSpotModel ByComain(List<CompanyHubSpotModel> companies, Organisation pexOrganisation, string domain, CompareResult result)
        {
            var company = companies.FirstOrDefault(company => company.Domain == domain);
            if (company != null) return company;

            result.Log("Not in hubspot", pexOrganisation.Id, pexOrganisation.Name);
            return null;
        }

        private string Domain(CompanyHubSpotModel pexOrganisation, CompareResult result)
        {
            return pexOrganisation.Domain;
        }

        private string Domain(Organisation pexOrganisation, CompareResult result)
        {
            var contacts = pexOrganisation.Contacts.Where(contact => contact.Type == ContactTypes.WebSite).ToList();
            switch (contacts.Count)
            {
                case 0:
                    result.Log("WithoutWebsite", pexOrganisation.Id, pexOrganisation.Name);
                    return null;

                case 1:
                    var url = new Uri(contacts.First().Value);
                    return url.Host;

                default:
                    result.Log("MoreAsOneWebsite", pexOrganisation.Id, pexOrganisation.Name);
                    return null;
            }
        }

        private async Task<IReadOnlyList<Organisation>> Organisatoins()
        {
            return await Session.Query<OrganisationView.Organisation>().ToListAsync();
        }

        private static List<ContactHubSpotModel> Contacts(HubSpotApi api)
        {
            var contacts = new List<ContactHubSpotModel>();
            var options = new ListRequestOptions {PropertiesToInclude = new List<string> {"firstname", "lastname", "email", "associatedcompanyid", "company"}};
            var result = api.Contact.List<ContactHubSpotModel>(options);

            do
            {
                contacts.AddRange(result.Contacts);
                options.Offset = result.ContinuationOffset;
                result = api.Contact.List<ContactHubSpotModel>(options);
            } while (result.MoreResultsAvailable);

            return contacts;
        }

        private static List<Company> Companies(HubSpotApi api)
        {
            var contacts = new List<Company>();
            var options = new ListRequestOptions {PropertiesToInclude = new List<string> { "name", "domain" }};
            var result = api.Company.List<Company>(options);

            do
            {
                contacts.AddRange(result.Companies);
                options.Offset = result.ContinuationOffset;
                result = api.Company.List<Company>(options);
            } while (result.MoreResultsAvailable);

            return contacts;
        }
    }

    internal class UpdateExistance
    {
    }
}