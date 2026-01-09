using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Commands
{
    public class SetOrganisationWarning : IRequest<Result>
    {
        public OrganisationId OrganisationId { get; }
        public UserId UserId { get; }
        public string Title { get; }
        public string Content { get; }

        public SetOrganisationWarning(OrganisationId organisationId, UserId userId, string title, string content)
        {
            OrganisationId = organisationId;
            UserId = userId;
            Title = title;
            Content = content;
        }
    }
}