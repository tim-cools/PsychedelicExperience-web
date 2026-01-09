using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands;
using PsychedelicExperience.ViewModels;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;
using Role = PsychedelicExperience.Membership.Messages.Users.Role;
using User = PsychedelicExperience.Membership.Messages.Users.User;

namespace PsychedelicExperience.Web.Controllers.Api
{
    public class AccountController : Controller
    {
        private readonly IMediator _messageDispatcher;

        public AccountController(IMediator messageDispatcher)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
        }

        [Authorize]
        [HttpGet("~/api/account/")]
        [ProducesResponseType(typeof(EmptyResponse), 403)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId();

            var query = new UserByIdQuery(userId, userId);
            var user = await _messageDispatcher.Send(query);

            return Json(user);
        }

        [UnauthenticateWhenSessionExpired]
        [HttpGet("~/api/account/{id}/")]
        [ProducesResponseType(typeof(EmptyResponse), 403)]
        [ProducesResponseType(typeof(User), 200)]
        public async Task<IActionResult> Get(UserId id)
        {
            var userId = User.GetUserId();

            var query = new UserByIdQuery(id, userId);
            var user = await _messageDispatcher.Send(query);

            return Json(user);
        }

        [AllowAnonymous]
        [HttpPost("~/api/account/register")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Register(UserModel userModel)
        {
            var fullName = !string.IsNullOrEmpty(userModel.FullName) ? new Name(userModel.FullName) : null;
            var name = new Name(userModel.DisplayName);
            var email = new EMail(userModel.EMail);
            var password = new Password(userModel.Password);
            var passwordConfirm = new Password(userModel.ConfirmPassword);

            var command = new RegisterUserCommand(fullName, name, email, password, passwordConfirm);
            var result = await _messageDispatcher.Send(command);

            return !result.Succeeded 
                ? ErrorResult(result) 
                : CreatedAtAction("Get", new { id = new ShortGuid(result.Id) }, null);
        }

        private IActionResult ErrorResult(Result result = null)
        {
            var errors = ModelState
                    .SelectMany(value => value.Value.Errors)
                    .Select(error =>new ValidationError(null, null, error.ErrorMessage))
                    .ToArray();

            var modelValidationResult = errors.Length == 0 ? Result.Success : Result.Failed(errors);

            var merged = result == null
                ? modelValidationResult
                : result.Merge(modelValidationResult);

            return merged.Succeeded ? (IActionResult)Ok() : new ObjectResult(merged) { StatusCode = StatusCodes.Status400BadRequest };
        }

        [Authorize]
        [HttpPost("~/api/account/{userId}/resend-confirm-email")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ResendConfirmEmail(UserId userId)
        {
            var currentUserId = User.GetUserId();

            var command = new RequestConfirmEmailCommand(currentUserId, userId);
            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/account/send-all-confirm-emails")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> SendAllConfirmEmail(string filter)
        {
            var currentUserId = User.GetUserId();

            var command = new RequestAllConfirmEmailCommand(currentUserId, filter);
            var result = await _messageDispatcher.Send(command);

            return this.ResultData(result);
        }

        [HttpPost("~/api/account/verify-invite")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> VerifyInvite(string token)
        {
            var currentUserId = User.GetUserId();

            var command = new VerifyOrganisationInvite(currentUserId, token);
            var result = await _messageDispatcher.Send(command);

            return this.ResultData(result);
        }

        [Authorize]
        [HttpPost("~/api/account/confirm-invite")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ConfirmInvite(string token)
        {
            var currentUserId = User.GetUserId();

            var command = new ConfirmOrganisationInvite(currentUserId, token);
            var result = await _messageDispatcher.Send(command);

            return this.ResultData(result);
        }

        [Authorize]
        [HttpPost("~/api/account/send-invites")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> SendAllInviteEmails(string filter)
        {
            var currentUserId = User.GetUserId();

            var command = new SendOrganisationInviteEmailsCommand(currentUserId, filter);
            var result = await _messageDispatcher.Send(command);

            return this.ResultData(result);
        }

        [Authorize]
        [HttpPost("~/api/account/{userId}/roles/{role}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddUserToRole(UserId userId, Role role)
        {
            var currentUserId = User.GetUserId();
            var command = new AddUserToRole(currentUserId, userId, role);
            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/account/{userId}/roles/{role}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveUserFromRole(UserId userId, Role role)
        {
            var currentUserId = User.GetUserId();
            var command = new RemoveUserFromRole(currentUserId, userId, role);
            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        //todo, this should send an email
        [HttpPost("~/api/account/request-reset-password")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RequestResetPassword(string email)
        {
            var command = new GenerateResetPasswordToken { UserEMail = email };
            var result = await _messageDispatcher.Send(command);

            return this.Result(result);
        }

        //todo, this should be used from page
        [HttpPost("~/api/account/reset-password")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> ResetPassword(UserId userId, string password, string token)
        {
            var command = new ResetPassword { UserId = userId, NewPassword = password, Token = token };
            var result = await _messageDispatcher.Send(command);

            return ErrorResult(result);
        }

        ////todo, this should be used from page(also support)
        ////not yet working, should implement IUserLockoutStore<User>
        ///// <summary>
        ///// Disable a user. (not yet working)
        ///// </summary>
        ///// <remarks>User should be Administrator</remarks>
        //[Authorize(Roles = RoleNames.Administrator)]
        //[HttpPost("~/api/account/disable")]
        //[ProducesResponseType(typeof(EmptyResponse), 200)]
        //[ProducesResponseType(typeof(Result), 400)]
        //public async Task<IActionResult> Disable(string email)
        //{
        //    var command = new DisableUser { UserEMail = email };
        //    var result = await _messageDispatcher.Send(command);

        //    return ErrorResult(result);
        //}

        ////todo, this should be used from page (also support)(
        ////not yet working, should implement IUserLockoutStore<User>
        ///// <summary>
        ///// Enable a user. (not yet working)
        ///// </summary>
        ///// <remarks>User should be Administrator</remarks>
        //[Authorize(Roles = RoleNames.Administrator)]
        //[HttpPost("~/api/account/enable")]
        //[ProducesResponseType(typeof(EmptyResponse), 200)]
        //[ProducesResponseType(typeof(Result), 400)]
        //public async Task<IActionResult> Enable(string email)
        //{
        //    var command = new EnableUser { UserEMail = email };
        //    var result = await _messageDispatcher.Send(command);

        //    return ErrorResult(result);
        //}
    }
}
