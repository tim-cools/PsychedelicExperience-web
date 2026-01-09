using System;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Membership.Messages.RefreshTokens
{
    public class DeleteRefreshTokenCommand : ICommand
    {
        public Guid TokenId { get; }

        public DeleteRefreshTokenCommand(Guid tokenId)
        {
            TokenId = tokenId;
        }

        public override string ToString()
        {
            return $"TokenId: {TokenId}";
        }
    }
}
