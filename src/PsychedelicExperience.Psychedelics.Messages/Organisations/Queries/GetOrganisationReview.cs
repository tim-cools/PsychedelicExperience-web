using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisationReview : IRequest<OrganisationReviewResult>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }
        public OrganisationReviewId ReviewId { get; }

        public GetOrganisationReview(UserId userId, OrganisationId organisationId, OrganisationReviewId reviewId)
        {
            UserId = userId;
            OrganisationId = organisationId;
            ReviewId = reviewId;
        }
    }
}
