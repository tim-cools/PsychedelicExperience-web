using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationReviewHealthcareProvider : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public HealthcareProviderReview Review { get; }

        public ChangeOrganisationReviewHealthcareProvider(OrganisationReviewId organisationReviewId, UserId userId, HealthcareProviderReview review)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Review = review;
        }
    }
}