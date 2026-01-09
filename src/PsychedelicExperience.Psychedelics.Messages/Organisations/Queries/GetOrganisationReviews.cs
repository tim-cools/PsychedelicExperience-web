using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisationReviews : IRequest<OrganisationReviewsResult>
    {
        public UserId UserId { get; }
        public string Query { get; }
        public string[] Tags { get; }
        public int Page { get; }

        public GetOrganisationReviews(UserId userId, string query = null, string[] tags = null, int page = 0)
        {
            UserId = userId;
            Query = query;
            Tags = tags;
            Page = page;
        }
    }
}