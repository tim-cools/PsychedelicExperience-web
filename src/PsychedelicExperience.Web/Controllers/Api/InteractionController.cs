using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class InteractionController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public InteractionController(IMediator messageDispatcher)
        {
            if (messageDispatcher == null) throw new ArgumentNullException(nameof(messageDispatcher));

            _messageDispatcher = messageDispatcher;
        }

        [HttpGet("~/api/interaction/{id}")]
        [ProducesResponseType(typeof(TopicInteractionDetails), 200)]
        public async Task<IActionResult> Get(TopicId id)
        {
            var userId = User.GetUserId();

            var query = new GetTopicInteraction(userId, id);
            var result = await _messageDispatcher.Send(query);

            return result != null ? Json(result) : id.AcceptedWhenRecent();
        }

        [Authorize]
        [HttpPut("~/api/interaction/{id}/follow")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Follow(TopicId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new Follow(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/interaction/{id}/like")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Like(TopicId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new Like(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/interaction/{id}/dislike")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Dislike(TopicId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new Dislike(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [HttpPut("~/api/interaction/{id}/view")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> View(TopicId id)
        {
            var userId = User.GetUserId();

            var query = new View(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/interaction/{id}/comment")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Comment(TopicId id, string text)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new Comment(userId, id, text);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpGet("~/api/interaction/{id}/followers")]
        [ProducesResponseType(typeof(TopicFollowersDetails), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Followers(TopicId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new GetTopicFollowers(userId, id);
            var result = await _messageDispatcher.Send(query);

            return Json(result);
        }
    }
}
