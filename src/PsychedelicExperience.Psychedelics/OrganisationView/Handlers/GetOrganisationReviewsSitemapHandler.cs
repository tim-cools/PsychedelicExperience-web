using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;

namespace PsychedelicExperience.Psychedelics.OrganisationView.Handlers
{
    public class GetOrganisationReviewsSitemapHandler : QueryHandler<GetOrganisationReviewsSitemap, OrganisationReviewsSitemapResult>
    {
        public GetOrganisationReviewsSitemapHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<OrganisationReviewsSitemapResult> Execute(GetOrganisationReviewsSitemap getOrganisationsQuery)
        {
            var reviews = await Session.Query<Review>()
                .Where(review => !review.Removed)
                .Select(organisation => new OrganisationReviewSitemapEntry
                {
                    OrganisationId = organisation.OrganisationId,
                    Id = organisation.Id,
                    Name = organisation.Name
                })
                .ToListAsync();

            return new OrganisationReviewsSitemapResult
            {
                Reviews = reviews.ToArray()
            };
        }
    }
}
