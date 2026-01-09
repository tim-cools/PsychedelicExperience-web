using System.Collections.Generic;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Membership.Messages.RefreshTokens
{
    public class RefreshTokensQuery : IRequest<IEnumerable<RefreshToken>>
    {
    }
}