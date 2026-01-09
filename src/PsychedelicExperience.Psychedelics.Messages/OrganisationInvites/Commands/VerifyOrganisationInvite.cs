using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands
{
    public class VerifyOrganisationInvite : IRequest<VerifyOrganisationInviteResult>
    {
        public UserId CurrentUserId { get; }
        public string Token { get; }

        public VerifyOrganisationInvite(UserId currentUserId, string token)
        {
            CurrentUserId = currentUserId;
            Token = token;
        }
    }
}