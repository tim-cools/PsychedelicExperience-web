using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RateOrganisationReview : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public ScaleOf5 Rating { get; }

        public RateOrganisationReview(OrganisationReviewId organisationReviewId, UserId userId, ScaleOf5 rating)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Rating = rating;
        }
    }
}