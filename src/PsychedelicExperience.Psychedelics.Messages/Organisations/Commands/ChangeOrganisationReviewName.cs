using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class ChangeOrganisationReviewName : IRequest<Result>
    {
        public OrganisationReviewId OrganisationReviewId { get; }
        public UserId UserId { get; }
        public string Name { get; }

        public ChangeOrganisationReviewName(OrganisationReviewId organisationReviewId, UserId userId, string name)
        {
            OrganisationReviewId = organisationReviewId;
            UserId = userId;
            Name = name;
        }
    }
}