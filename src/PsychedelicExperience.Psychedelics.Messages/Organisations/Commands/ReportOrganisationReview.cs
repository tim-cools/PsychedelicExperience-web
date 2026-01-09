using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ReportOrganisationReview : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public string Reason { get; }

        public ReportOrganisationReview(OrganisationId organisationId, OrganisationReviewId organisationReviewId, UserId userId, string reason)
        {
            OrganisationId = organisationId;
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Reason = reason;
        }
    }
}