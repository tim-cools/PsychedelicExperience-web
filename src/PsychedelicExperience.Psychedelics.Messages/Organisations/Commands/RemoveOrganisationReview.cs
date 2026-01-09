using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class RemoveOrganisationReview : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }

        public RemoveOrganisationReview(OrganisationReviewId organisationReviewId, UserId userId)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
        }
    }
}