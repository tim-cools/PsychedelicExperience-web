using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.RefreshTokens
{
    public class RefreshTokenValidator : IRequest<Result>
    {
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }

        public RefreshTokenValidator(string refreshToken, string clientId, string userId)
        {
            RefreshToken = refreshToken;
            ClientId = clientId;
            UserId = userId;
        }
    }
}