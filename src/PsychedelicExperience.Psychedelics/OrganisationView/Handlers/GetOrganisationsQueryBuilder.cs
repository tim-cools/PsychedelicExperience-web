using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Baseline;
using LinqKit;
using Marten;
using Marten.Linq;
using Marten.Services.Includes;
using MemBus.Support;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationsQueryBuilder
    {
        public const int PageSize = 20;

        private ExpressionStarter<Organisation> _where;
        private readonly IQuerySession _session;
        private readonly IUserInfoResolver _userInfoResolver;
        private int _page;
        private User _user;
        private Format _format;

        public GetOrganisationsQueryBuilder(IQuerySession session, IUserInfoResolver userInfoResolver)
        {
            _session = session;
            _userInfoResolver = userInfoResolver;
            _where = PredicateBuilder.New<Organisation>();
        }

        public GetOrganisationsQueryBuilder FilterCountry(string country)
        {
            if (!string.IsNullOrWhiteSpace(country))
            {
                country = country.NormalizeForSearch();

                _where = _where.And(experience => experience.Address.Country == country);
            }
            return this;
        }

        public GetOrganisationsQueryBuilder WithPrivacy(User user)
        {
            _user = user;
            _where = _where.And(experience => !experience.Removed);

/*
            //todo add function (criteria option) to view removed organisation
            if (user == null || !user.IsAdministrator())
            {
                _where = _where.And(experience => !experience.Removed);
            }
            else
            {
                _where = _where.And(experience => experience.Removed || !experience.Removed);
            }*/

            return this;
        }
        
        public GetOrganisationsQueryBuilder FilterTypes(params string[] types)
        {
            if (types != null && types.Length > 0)
            {
                var typesWhere = PredicateBuilder.New<Organisation>();
                foreach (var type in types)
                {
                    typesWhere = typesWhere.Or(experience => experience.Types.Contains(type));
                }

                _where = _where.And(typesWhere);
            }
            return this;
        }

        public GetOrganisationsQueryBuilder FilterByUser(bool filter, User user)
        {
            if (filter && user != null)
            {
                var userId = user.Id;
                _where = _where.And(organisation => organisation.Owners.Contains(userId));
            }
            return this;
        }

        public GetOrganisationsQueryBuilder FilterByHasOwner(bool? filter)
        {
            switch (filter)
            {
                case true:
                    //todo this one doesn't seem to work?!
                    _where = _where.And(organisation => organisation.Owners.Count != 0);
                    break;

                case false:
                    _where = _where.And(organisation => organisation.Owners.Count == 0);
                    break;
            }

            return this;
        }

        public GetOrganisationsQueryBuilder FilterQueryString(string queryString)
        {
            queryString = queryString.NormalizeForSearch();

            if (string.IsNullOrWhiteSpace(queryString)) return this;

            var query = PredicateBuilder.New<Organisation>()
                .Or(organisation => organisation.SearchString.Contains(queryString))
                .Or(organisation => organisation.TagsNormalized.Contains(queryString));

            _where = _where.And(query);

            return this;
        }

        public GetOrganisationsQueryBuilder FilterTags(string[] tags, bool onlyWithoutTags)
        {
            if (onlyWithoutTags)
            {
                _where = _where.And(organisation => organisation.TagsNormalized.Count <= 1);
                return this;
            }

            if (tags == null || tags.Length == 0) return this;

            foreach (var tag in tags)
            {
                var value = tag.NormalizeForSearch();
                _where = _where.And(organisation => organisation.TagsNormalized.Contains(value));
            }
            return this;
        }

        public GetOrganisationsQueryBuilder Paging(int page)
        {
            _page = page;
            return this;
        }

        private OrganisationSummary[] Map(IReadOnlyList<Organisation> organisations, List<TopicInteraction> interactions, User user)
        {
            return organisations
                .Select(value => new
                {
                    Organisation = value,
                    Interaction = interactions.FirstOrDefault(interaction => interaction?.Id == value.Id)
                })
                .Select(value => value.Organisation.MapSummary(user, value.Interaction, _userInfoResolver))
                .ToArray();
        }

        public async Task<OrganisationsResult> Execute()
        {
            var stats = new QueryStatistics();

            var interactions = new List<TopicInteraction>();

            var skip = _page >= 0 ? _page * PageSize : 0;
            var take = _page >= 0 ? PageSize : int.MaxValue;

            var query = _session.Query<Organisation>()
                .Stats(out stats)
                .Include(organisation => organisation.Id, interactions, JoinType.LeftOuter)
                .Where(_where)
                .OrderByDescending(organisation => organisation.PhotosCount == 0)
                .OrderBy(organisation => organisation.Name)
                .Skip(skip)
                .Take(take);

            var organisations = await query.ToListAsync();

            return _format == Messages.Format.Json
                ? Json(organisations, interactions, stats, skip)
                : Csv(organisations, interactions, stats, skip);
        }

        private OrganisationsResult Json(IReadOnlyList<Organisation> organisations, List<TopicInteraction> interactions, QueryStatistics stats, int skip)
        {
            return new JsonOrganisationsResult
            {
                Organisations = Map(organisations, interactions, _user),
                Page = _page,
                Total = stats.TotalResults,
                Last = skip + organisations.Count
            };
        }

        private OrganisationsResult Csv(IReadOnlyList<Organisation> organisations, List<TopicInteraction> interactions, QueryStatistics stats, int skip)
        {
            var writer = new CsvWriter();
            foreach (var organisation in organisations)
            {
                AddOrganisation(writer, organisation);
            }

            return new CsvOrganisationsResult
            {
                Bytes = Encoding.Default.GetBytes(writer.ToString()),
                FileName = $"PEx-Organisations-{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv",
            };
        }

        private static void AddOrganisation(CsvWriter csvWriter, Organisation organisation)
        {
            csvWriter.Write(nameof(Organisation.Id), organisation.Id);
            csvWriter.Write(nameof(Organisation.Owners), organisation.Owners.Count);
            csvWriter.Write(nameof(Organisation.Person), organisation.Person);
            csvWriter.Write(nameof(Organisation.Name), organisation.Name);
            AddNestedObject(csvWriter, nameof(Organisation.Address), organisation.Address);
            csvWriter.Write(nameof(Organisation.Description), organisation.Description);
            csvWriter.Write(nameof(Organisation.Country), organisation.Country);
            csvWriter.Write(nameof(Organisation.Contacts), organisation.Contacts.Count);
            AddContacts(csvWriter, nameof(Organisation.Contacts), organisation.Contacts);
            AddNestedObject(csvWriter, nameof(Organisation.Info), organisation.Info);
            AddNestedObject(csvWriter, nameof(Organisation.Warning), organisation.Warning);
            csvWriter.Write(nameof(Organisation.Removed), organisation.Removed);
            csvWriter.Write(nameof(Organisation.Reviews), organisation.Reviews.Count);
            csvWriter.Write(nameof(Organisation.Reports), organisation.Reports.Count);
            csvWriter.Write(nameof(Organisation.Photos), organisation.Photos.Count);
            csvWriter.Write(nameof(Organisation.Locations), organisation.Locations.Count);
            csvWriter.Write(nameof(Organisation.Relations), organisation.Relations.Count);
            csvWriter.Write(nameof(Organisation.RelationsFrom), organisation.RelationsFrom.Count);
            csvWriter.Write(nameof(Organisation.Created), organisation.Created);
            csvWriter.Write(nameof(Organisation.LastUpdated), organisation.LastUpdated);
            AddNestedObject(csvWriter, nameof(Organisation.HealthcareProvider), organisation.HealthcareProvider);
            AddNestedObject(csvWriter, nameof(Organisation.Community), organisation.Community);
            AddNestedObject(csvWriter, nameof(Organisation.Center), organisation.Center);
            AddNestedObject(csvWriter, nameof(Organisation.Practitioner), organisation.Practitioner);
            AddStringColumns(csvWriter, nameof(Organisation.Types), organisation.Types);
            AddStringColumns(csvWriter, nameof(Organisation.Tags), organisation.Tags);
            csvWriter.EndLine();
        }

        private static void AddStringColumns(CsvWriter csvWriter, string parent, IList<string> values)
        {
            foreach (var value in values.Distinct())
            {
                var propertyName = $"{parent}/{value}";
                csvWriter.WriteSorted(propertyName, true);
            }
        }

        private static void AddContacts(CsvWriter csvWriter, string parent, IList<Contact> contacts)
        {
            var counters = new ConcurrentDictionary<string, int>();
            foreach (var contact in contacts)
            {
                var counter = counters.AddOrUpdate(contact.Type, 1, (key, value) => value + 1);
                var propertyName = $"{parent}/{contact.Type}/{counter}";

                csvWriter.WriteSorted(propertyName, contact.Value);
            }
        }

        private static void AddNestedObject(CsvWriter csvWriter, string parent, Object value)
        {
            if (value == null) return;
            var properties = value.GetType().GetProperties();

            foreach (var property in properties)
            {
                AddProperty(csvWriter, parent, value, property);
            }
        }

        private static void AddProperty(CsvWriter csvWriter, string parent, object value, PropertyInfo property)
        {
            try
            {
                var propertyValue = property.GetValue(value);
                var propertyName = $"{parent}/{property.Name}";
                if (property.PropertyType.IsArray)
                {
                    var arrayValue = propertyValue as IEnumerable;
                    var stringValues = arrayValue
                        .Cast<object>()
                        .Select(value => value?.ToString())
                        .ToArray();
                    AddStringColumns(csvWriter, propertyName, stringValues);
                }
                else if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    csvWriter.Write(propertyName, propertyValue);
                }
                else
                {
                    AddNestedObject(csvWriter, propertyName, propertyValue);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not add property: " + property.Name, ex);
            }
        }

        public GetOrganisationsQueryBuilder Format(Format format)
        {
            _format = format;
            return this;
        }
    }
}