using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using PsychedelicExperience.Membership.Messages.Users;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;
using RefreshToken = PsychedelicExperience.Membership.RefreshTokens.Domain.RefreshToken;

namespace PsychedelicExperience.Membership.RefreshTokens.Handlers
{
    public class DeleteRefreshTokenHandler : CommandHandler<DeleteRefreshTokenCommand>
    {
        private readonly ILogger<DeleteRefreshTokenHandler> _logger;

        public DeleteRefreshTokenHandler(IDocumentSession session, ILogger<DeleteRefreshTokenHandler> logger) : base(session)
        {
            _logger = logger;
        }

        protected override async Task<Result> Execute(DeleteRefreshTokenCommand command)
        {
            var token = await Session.LoadAsync<RefreshToken>(command.TokenId);
            if (token == null)
            {
                return Result.Failed("RefreshToken", ErrorCodes.RefreshTokenInvalid, "Could not find token");
            }

            Session.Delete(token);

            _logger.LogInformation($"Refresh Token deleted: {command}");

            return Result.Success;
        }
    }
}