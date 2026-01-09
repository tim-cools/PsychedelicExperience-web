using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationHandler : QueryHandler<GetOrganisation, OrganisationDetails>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetOrganisationHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<OrganisationDetails> Execute(GetOrganisation query)
        {
            if (query.OrganisationId == null) throw new InvalidOperationException("OrganisationId is null");
            
            var id = (Guid) query.OrganisationId;

            var organisation = await Session.LoadAsync<Organisation>(id);
            if (organisation == null)
            {
                return null;
            }

            var reviews = await Session.Query<Review>()
                .Where(review => review.OrganisationId == id && !review.Removed)
                .ToListAsync();

            var user = await Session.LoadUserAsync(query.UserId);

            var relatedOrganisationIds = organisation.RelationOrganisationIds();

            var relatedOrganisations = await Session.LoadManyAsync<Organisation>(relatedOrganisationIds);

            var relatedOrganisationDictionary = relatedOrganisations.ToDictionary(value => value.Id, value => value);

            return organisation.MapDetails(user, reviews, relatedOrganisationDictionary, _userInfoResolver);
        }
    }
}
