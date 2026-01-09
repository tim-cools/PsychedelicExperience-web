using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using RefreshToken = PsychedelicExperience.Membership.RefreshTokens.Domain.RefreshToken;
using System.Linq;
using Marten.Schema.Identity;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;

namespace PsychedelicExperience.Membership.RefreshTokens.Handlers
{
    public class CreateRefreshTokenHandler : CommandHandler<CreateRefreshTokenCommand>
    {
        private readonly ILogger<CreateRefreshTokenHandler> _logger;

        public CreateRefreshTokenHandler(IDocumentSession session, ILogger<CreateRefreshTokenHandler> logger) : base(session)
        {
            _logger = logger;
        }

        protected override Task<Result> Execute(CreateRefreshTokenCommand command)
        {
            var hash = Hasher.ComputeSHA256(command.RefreshToken);

            VerifyNotExisting(hash);

            var token = new RefreshToken
            {
                Id = CombGuidIdGeneration.NewGuid(),
                Hash = hash,
                ClientId = command.ClientId,
                UserId = command.UserId,
                IpAddress = command.IpAddress,
                ExpiresUtc = command.ExpiresUtc,
                IssuedUtc = command.IssuedUtc
            };

            Session.Store(token);

            _logger.LogInformation($"Refresh Token Created: {command}");

            return Task.FromResult(Result.Success);
        }

        private void VerifyNotExisting(string hash)
        {
            var existing = Session.Query<RefreshToken>().FirstOrDefault(criteria => criteria.Hash == hash);
            if (existing != null)
            {
                var error = new ValidationError("RefreshToken", ErrorCodes.RefreshTokenAlreadyExists, "Create refresh token failed");
                throw new BusinessException(error);
            }
        }
    }
}