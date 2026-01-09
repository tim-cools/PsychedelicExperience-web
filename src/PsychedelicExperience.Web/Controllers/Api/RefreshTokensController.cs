using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using PsychedelicExperience.Membership.Users.Domain;

namespace PsychedelicExperience.Web.Controllers.Api
{
    public class RefreshTokensController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public RefreshTokensController(IMediator messageDispatcher)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));

            _messageDispatcher = messageDispatcher;
        }

        //[Authorize()]
        //[HttpGet("api/RefreshTokens")]
        //public async Task<IActionResult> Get()
        //{
        //    var query = new RefreshTokensQuery();
        //    var tokens = await _messageDispatcher.Send(query);

        //    return Ok(tokens);
        //}

        //[Authorize()]
        //[HttpDelete("api/RefreshTokens")]
        //public async Task<IActionResult> Delete(Guid tokenId)
        //{
        //    var command = new DeleteRefreshTokenCommand(tokenId);
        //    var result = await _messageDispatcher.Send(command);

        //    return result.Succeeded ? (IActionResult) Ok() : BadRequest("Token Id does not exist");
        //}
    }
}
