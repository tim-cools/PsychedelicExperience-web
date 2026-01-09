using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationReviewDescription : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public string Description { get; }

        public ChangeOrganisationReviewDescription(OrganisationReviewId organisationReviewId, UserId userId, string description)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Description = description;
        }
    }
}