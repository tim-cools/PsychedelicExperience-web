using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class OrganisationUpdatesController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public OrganisationUpdatesController(IMediator messageDispatcher)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));

            _messageDispatcher = messageDispatcher;
        }
        
        [Authorize]
        [HttpPost("~/api/organisation/{id}/update")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddUpdate(OrganisationId id, string subject, string content, OrganisationUpdatePrivacy privacy)
        {
            var updateId = OrganisationUpdateId.New();
            var userId = User.GetAuthorizedUserId();

            var command = new AddOrganisationUpdate(userId, id, updateId, subject, content, privacy);

            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return CreatedAtAction("GetUpdate", new { id = new ShortGuid(updateId.Value) }, null);
        }

        [HttpGet("~/api/organisation/{id}/update")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> GetUpdates(OrganisationId id, int page = 0)
        {
            var userId = User.GetUserId();

            var request = new GetOrganisationUpdates(userId, id, false, page);
            var result = await _messageDispatcher.Send(request);

            return Json(result);
        }

        [HttpGet("~/api/organisation/{id}/update/{updateId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> GetUpdate(OrganisationId id, OrganisationUpdateId updateId)
        {
            var userId = User.GetUserId();

            var request = new GetOrganisationUpdate(userId, id, updateId, false);
            var result = await _messageDispatcher.Send(request);

            return result != null ? Json(result) : updateId.AcceptedWhenRecent();
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/update/{updateId}/update-subject")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateName(OrganisationId id, OrganisationUpdateId updateId, string subject)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetOrganisationUpdateSubject(userId, id, updateId, subject);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/update/{updateId}/update-content")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateContent(OrganisationId id, OrganisationUpdateId updateId, string content)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetOrganisationUpdateContent(userId, id, updateId, content);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/organisation/{id}/update/{updateId}/update-privacy")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdatePrivacy(OrganisationId id, OrganisationUpdateId updateId, OrganisationUpdatePrivacy privacy)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new SetOrganisationUpdatePrivacy(userId, id, updateId, privacy);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/organisation/{id}/update/{updateId}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveUpdate(OrganisationId id, OrganisationUpdateId updateId)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveOrganisationUpdate(userId, id, updateId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }
    }
}
