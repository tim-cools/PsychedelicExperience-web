using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using Marten.Linq;
using Marten.Services.Includes;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.TopicInteractionView;

namespace PsychedelicExperience.Psychedelics.ExperienceView.Handlers
{
    public class GetExperiencesQueryBuilder
    {
        public const int PageSize = 20;

        private ExpressionStarter<Experience> _where;
        private readonly IQuerySession _session;
        private readonly IUserInfoResolver _userInfoResolver;
        private int _page;

        public GetExperiencesQueryBuilder(IQuerySession session, IUserInfoResolver userInfoResolver)
        {
            _session = session;
            _userInfoResolver = userInfoResolver;
            _where = PredicateBuilder.New<Experience>();
        }

        public GetExperiencesQueryBuilder WithPrivacy(UserId userId)
        {
            if (userId != null)
            {
                var value = userId.Value;
                _where = _where.And(experience => experience.PrivacyLevel == PrivacyLevel.Public
                                               || experience.UserId == value);
            }
            else
            {
                _where = _where.And(experience => experience.PrivacyLevel == PrivacyLevel.Public);
            }
            return this;
        }

        public GetExperiencesQueryBuilder FilterByUser(bool filter, UserId userdId)
        {
            if (filter && userdId != null)
            {
                var id = (Guid) userdId;
                _where = _where.And(organisation => organisation.UserId == id);
            }
            return this;
        }

        public GetExperiencesQueryBuilder FilterQueryString(string queryString)
        {
            queryString = queryString.NormalizeForSearch();

            if (string.IsNullOrWhiteSpace(queryString)) return this;

            var query = PredicateBuilder.New<Experience>()
                .Or(experience => experience.SearchString.Contains(queryString))
                .Or(experience => experience.Doses.Any(dose => dose.SubstanceNormalized == queryString))
                .Or(experience => experience.TagsNormalized.Contains(queryString));

            _where = _where.And(query);

            return this;
        }

        public GetExperiencesQueryBuilder FilterTags(string[] tags)
        {
            if (tags == null || tags.Length == 0) return this;

            foreach (var tag in tags)
            {
                var value = tag.NormalizeForSearch();
                _where = _where.And(experience => experience.TagsNormalized.Contains(value));
            }
            return this;
        }

        public GetExperiencesQueryBuilder FilterSubstance(string[] substances)
        {
            if (substances == null || substances.Length == 0) return this;

            foreach (var substance in substances)
            {
                var value = substance.NormalizeForSearch();
                _where = _where.And(experience => experience.Doses.Any(dose => dose.SubstanceNormalized == value));
            }
            return this;
        }

        public GetExperiencesQueryBuilder Paging(int page)
        {
            _page = page;
            return this;
        }

        private ExperienceSummary[] Map(IReadOnlyList<Experience> experiences, List<TopicInteraction> interactions)
        {
            return experiences
                .Select(value => new
                {
                    Experience = value,
                    Interaction = interactions.FirstOrDefault(interaction => interaction?.Id == value.Id)
                })
                .Select(value => value.Experience.MapSummary(value.Interaction, _userInfoResolver))
                .ToArray();
        }

        public async Task<ExperiencesResult> Execute()
        {
            QueryStatistics stats;

            var interactions = new List<TopicInteraction>();

            var skip = _page*PageSize;
            var query = _session.Query<Experience>()
                .Stats(out stats)
                .Include(experience => experience.Id, interactions, JoinType.LeftOuter)
                .Where(_where)
                .OrderBy(experience => experience.Created)
                .Skip(skip)
                .Take(PageSize);

            var experiences = await query.ToListAsync();

            return new ExperiencesResult
            {
                Experiences = Map(experiences, interactions),
                Page = _page,
                Total = stats.TotalResults,
                Last = skip + experiences.Count
            };
        }
    }
}