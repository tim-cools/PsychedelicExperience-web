using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.RefreshTokens;

namespace PsychedelicExperience.Membership.RefreshTokens.Handlers
{
    public class RefreshTokensQueryHandler : QueryHandler<RefreshTokensQuery, IEnumerable<RefreshToken>>
    {

        public RefreshTokensQueryHandler(IQuerySession session)
              : base(session)
        {
        }

        protected override Task<IEnumerable<RefreshToken>> Execute(RefreshTokensQuery query)
        {
            IEnumerable<RefreshToken> tokens = Session.Query<Domain.RefreshToken>()
                .ToArray()
                .Select(token => new RefreshToken
                {
                    Id = token.Id,
                    IpAddress = token.IpAddress, 
                    ClientId = token.ClientId,
                    ExpiresUtc = token.ExpiresUtc,
                    IssuedUtc = token.IssuedUtc,
                    UserId = token.UserId
                })
                .ToArray();
            return Task.FromResult(tokens);
        }
    }
}