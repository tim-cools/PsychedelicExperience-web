using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using System.Linq;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;

namespace PsychedelicExperience.Membership.RefreshTokens.Handlers
{
    public class RefreshTokenValidatorHandler : QueryHandler<RefreshTokenValidator, Result>
    {
        public RefreshTokenValidatorHandler(IQuerySession session)
              : base(session)
        {
        }

        protected override Task<Result> Execute(RefreshTokenValidator query)
        {
            var result = Validate(query);

            return Task.FromResult(result);
        }

        private Result Validate(RefreshTokenValidator query)
        {
            var hash = Hasher.ComputeSHA256(query.RefreshToken);

            var token = Session.Query<Domain.RefreshToken>().FirstOrDefault(criteria => criteria.Hash == hash);
            if (token == null)
            {
                return Result.Failed("RefreshToken", ErrorCodes.RefreshTokenInvalid, "Unknown refresh token");
            }

            if (token.UserId != query.UserId || token.ClientId != query.ClientId)
            {
                return Result.Failed("RefreshToken", Messages.ErrorCodes.RefreshTokenInvalid, "Invalid refresh token");
            }

            return Result.Success;
        }
    }
}