using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationReviewCommunity : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public CommunityReview Review { get; }

        public ChangeOrganisationReviewCommunity(OrganisationReviewId organisationReviewId, UserId userId, CommunityReview review)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Review = review;
        }
    }
}