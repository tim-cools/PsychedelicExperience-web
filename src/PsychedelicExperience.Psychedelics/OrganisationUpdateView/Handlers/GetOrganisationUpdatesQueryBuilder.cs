using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Marten;
using Marten.Linq;
using Marten.Services.Includes;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.OrganisationView.Handlers;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdateView.Handlers
{
    public class GetOrganisationUpdatesQueryBuilder
    {
        public const int PageSize = 20;

        private readonly IQuerySession _session;
        private readonly IUserInfoResolver _userInfoResolver;
        private ExpressionStarter<OrganisationUpdate> _where;
        private int _page;
        private User _user;
        private bool _includeOrganisation;
        private Guid _origanisationIdValue;

        public GetOrganisationUpdatesQueryBuilder(IQuerySession session, IUserInfoResolver userInfoResolver)
        {
            _session = session;
            _userInfoResolver = userInfoResolver;
            _where = PredicateBuilder.New<OrganisationUpdate>();
        }

        public GetOrganisationUpdatesQueryBuilder IncludeOrganisation(bool includeOrganisation)
        {
            _includeOrganisation = includeOrganisation;
            return this;
        }

        public GetOrganisationUpdatesQueryBuilder WithPrivacy(User user, bool isMember, bool isOwner)
        {
            _user = user;
            if (user != null && (user.IsAdministrator() || isMember || isOwner))
            {
                _where = _where.And(update => !update.Removed);
            }
            else
            {
                _where = _where.And(update => !update.Removed && update.Privacy == OrganisationUpdatePrivacy.Public);
            }
            return this;
        }

        public GetOrganisationUpdatesQueryBuilder ForOrganisation(OrganisationId origanisationId)
        {
            _origanisationIdValue = origanisationId.Value;
            _where = _where.And(update => update.OrganisationId == _origanisationIdValue);
            return this;
        }

        public GetOrganisationUpdatesQueryBuilder Paging(int page)
        {
            _page = page;
            return this;
        }

        public async Task<OrganisationUpdatesResult> Execute()
        {
            var stats = new QueryStatistics();

            var skip = _page * PageSize;

            var organisations = new List<Organisation>();

            var updates = await _session.Query<OrganisationUpdate>()
                .Stats(out stats)
                .Where(_where)
                .Include(update => update.OrganisationId, organisations, JoinType.LeftOuter)
                .OrderByDescending(update => update.Created)
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();

            var organisation = await GetOrganisation();

            return new OrganisationUpdatesResult
            {
                Organisation = organisation,
                Updates = Map(updates, organisations, _user),
                Page = _page,
                Total = stats.TotalResults,
                Last = skip + updates.Count
            };
        }

        private async Task<OrganisationDetails> GetOrganisation()
        {
            if (!_includeOrganisation) return null;

            var organisation = await _session.LoadAsync<Organisation>(_origanisationIdValue);

            var reviews = await _session.Query<Review>()
                .Where(review => review.OrganisationId == _origanisationIdValue && !review.Removed)
                .ToListAsync();

            return organisation?.MapDetails(_user, reviews, null, _userInfoResolver);

        }

        private OrganisationUpdateSummary[] Map(IReadOnlyList<OrganisationUpdate> updates, List<Organisation> organisations, User user)
        {
            return updates
                .Select(value => new
                {
                    Update = value,
                    Organisation = organisations.FirstOrDefault(organisation => organisation?.Id == value.OrganisationId)
                })
                .Select(value => value.Update.MapSummary(user, value.Organisation, _userInfoResolver))
                .ToArray();
        }
    }
}