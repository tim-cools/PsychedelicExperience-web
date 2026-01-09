using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.RefreshTokens
{
    public class CreateRefreshTokenCommand : IRequest<Result>
    {
        public string RefreshToken { get; }

        public string ClientId { get; }
        public string UserId { get; }
        public string UserName { get; set; }

        public string IpAddress { get; }

        public DateTimeOffset ExpiresUtc { get; }
        public DateTimeOffset IssuedUtc { get; set; }

        public CreateRefreshTokenCommand(string refreshToken, string clientId, string userId, string userName, string ipAddress, DateTimeOffset issuedUtc, DateTimeOffset expiresUtc)
        {
            RefreshToken = refreshToken;
            ClientId = clientId;
            UserId = userId;
            UserName = userName;
            IpAddress = ipAddress;
            IssuedUtc = issuedUtc;
            ExpiresUtc = expiresUtc;
        }

        public override string ToString()
        {
            return $"ClientId: {ClientId}, UserId: {UserId}, UserName: {UserName}, IpAddress: {IpAddress}, ExpiresUtc: {ExpiresUtc}, IssuedUtc: {IssuedUtc}";
        }
    }
}
