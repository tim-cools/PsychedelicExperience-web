using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationReviewCenter : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public CenterReview Review { get; }

        public ChangeOrganisationReviewCenter(OrganisationReviewId organisationReviewId, UserId userId, CenterReview review)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Review = review;
        }
    }
}