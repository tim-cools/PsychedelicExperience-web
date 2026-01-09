using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands
{
    public class SendOrganisationInviteEmailsCommand : IRequest<ContentResult>
    {
        public UserId CurrentUserId { get; }
        public string Filter { get; }

        public SendOrganisationInviteEmailsCommand(UserId currentUserId, string filter)
        {
            CurrentUserId = currentUserId;
            Filter = filter;
        }
    }
}