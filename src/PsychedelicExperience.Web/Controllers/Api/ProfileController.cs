using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Psychedelics.Messages.Notifications;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class ProfileController : Controller
    {
        private readonly IMediator _messageDispatcher;
        private readonly IConfiguration _configuration;

        public ProfileController(IMediator messageDispatcher, IConfiguration configuration)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [Authorize]
        [HttpGet("~/api/profile/")]
        [ProducesResponseType(typeof(EmptyResult), 403)]
        [ProducesResponseType(typeof(UserProfileDetails), 200)]
        public async Task<IActionResult> GetCurrent()
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UserProfileByIdQuery(new UserProfileId(userId.Value), userId);
            var user = await _messageDispatcher.Send(query);

            return Json(user);
        }

        [Authorize]
        [HttpGet("~/api/profile/notifications")]
        [ProducesResponseType(typeof(NotificationsResult), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Notifications(int page)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new GetNotifications(userId, page);
            var result = await _messageDispatcher.Send(query);

            return Json(result);
        }

        [HttpGet("~/api/profile/{id}/")]
        [ProducesResponseType(typeof(EmptyResult), 403)]
        [ProducesResponseType(typeof(UserProfileDetails), 200)]
        public async Task<IActionResult> Get(UserProfileId id)
        {
            var userId = User.GetUserId();

            var query = new UserProfileByIdQuery(id, userId);
            var user = await _messageDispatcher.Send(query);

            return user != null ? Json(user) : id.AcceptedWhenRecent();
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-email")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeEMail(UserProfileId id, EMail email)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileEMail(userId, id, email);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-address")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeAddress(UserProfileId id, GooglePlace address)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileAddress(userId, id, address.ToAddress());
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-display-name")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeDisplayName(UserProfileId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileDisplayName(userId, id, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-full-name")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeFullName(UserProfileId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileFullName(userId, id, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/profile/{id}/change-avatar")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeAvatar(UserProfileId id, ICollection<IFormFile> files)
        {
            var userId = User.GetAuthorizedUserId();

            if (files.Count != 1)
            {
                return BadRequest("Request should contain 1 file");
            }

            var file = files.First();
            var savedFile = await file.Save(_configuration);
            var query = new ChangeUserProfileAvatar(userId, id, savedFile);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-tagline")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeTagline(UserProfileId id, string tagLine)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileTagline(userId, id, tagLine);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-description")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangeDescription(UserProfileId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileDescription(userId, id, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/change-privacy")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ChangePrivacy(UserProfileId id, PrivacyLevel level)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfilePrivacy(userId, id, level);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/notification-email/enable")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> NotificationEmailEnable(UserProfileId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new EnableUserProfileNotificationEmail(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/notification-email/disable")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> NotificationEmailDisable(UserProfileId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new DisableUserProfileNotificationEmail(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/profile/{id}/notification-email/interval")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> NotificationEmailChangeInterval(UserProfileId id, int minutes)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeUserProfileNotificationEmailInterval(userId, id, TimeSpan.FromMinutes(minutes));
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }
    }
}
