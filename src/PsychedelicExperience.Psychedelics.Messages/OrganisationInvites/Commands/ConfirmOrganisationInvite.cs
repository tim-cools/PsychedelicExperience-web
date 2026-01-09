using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands
{
    public class ConfirmOrganisationInvite : IRequest<ConfirmOrganisationInviteResult>
    {
        public UserId CurrentUserId { get; }
        public string Token { get; }

        public ConfirmOrganisationInvite(UserId currentUserId, string token)
        {
            CurrentUserId = currentUserId;
            Token = token;
        }
    }
}