using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;
using System.Linq;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Services;
using PsychedelicExperience.Membership.UserContactLog;
using PsychedelicExperience.Membership.UserProfiles;
using PsychedelicExperience.Membership.Users.Domain;
using Role = PsychedelicExperience.Membership.Messages.Users.Role;

namespace PsychedelicExperience.Membership.Users.Handlers
{
    public class EnsureAdministratorHandler : CommandHandler<EnsureAdministrator, Result>
    {
        public EnsureAdministratorHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(EnsureAdministrator command)
        {
            var currentUser = await Session.LoadUserAsync(command.CurrentUserId);
            if (currentUser == null)
            {
                return Result.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Current User not found.");
            }

            if (!currentUser.IsAdministrator())
            {
                return Result.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Not allowed.");
            }

            return Result.Success;
        }
    }

    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        const string lettersAndNumbers = "^[\\w0-9_ ]+$";

        public RegisterUserValidator()
        {
            RuleFor(customer => customer.Password)
                .Equal(customer => customer.PasswordConfirm)
                .WithMessage("Those passwords didn't match. Try again.");

            RuleFor(command => command.DisplayName)
                .Matches(lettersAndNumbers).WithMessage("Name should only contain letters, numbers, underscore and spaces.")
                .Length(1, 50).WithMessage("Name should be between 1 and 50 character.")
                .NotEmpty()
                .NotNull();

            RuleFor(command => command.FullName)
                .Matches(lettersAndNumbers).WithMessage("Name should only contain letters, numbers, underscore and spaces")
                .Length(3, 50).WithMessage("Name should be between 3 and 50 character.");

            RuleFor(command => command.EMail)
                .Length(3, 50).WithMessage("Email should be between 1 and 255 character.")
                .NotEmpty()
                .NotNull()
                .Email();
        }
    }

    public class RegisterUserHandler : CommandHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailSender _mailSender;
        private readonly IConfiguration _configuration;
        private readonly IValidator<RegisterUserCommand> _validatorCommand;

        public RegisterUserHandler(UserManager<User> userManager, IDocumentSession session, IMailSender mailSender, IConfiguration configuration, IValidator<RegisterUserCommand> validatorCommand) : base(session)
        {
            _userManager = userManager;
            _mailSender = mailSender;
            _configuration = configuration;
            _validatorCommand = validatorCommand;
        }

        protected override async Task<RegisterUserResult> Execute(RegisterUserCommand command)
        {
            if (!Validate(command, out var errors)) return errors;

            var user = new User(command.DisplayName.Value, command.EMail.Value);

            var result = await _userManager.CreateAsync(user, command.Password.Value);
            if (!result.Succeeded)
            {
                return new RegisterUserResult(result.ValidationErrors());
            }

            var @event = new UserRegistered(
                new UserId(user.Id),
                null,
                LoginType.UserName,
                command.FullName,
                command.DisplayName,
                command.EMail,
                DateTime.Now
                );

            Session.Events.Append(user.Id, @event);

            AddUserProfile(@event);

            await SendConfirmEmail(user);

            return new RegisterUserResult(user.Id);
        }

        private bool Validate(RegisterUserCommand command, out RegisterUserResult errors)
        {
            var result = _validatorCommand.Validate(command);
            if (!result.IsValid)
            {
                errors = new RegisterUserResult(result.Errors.Select(Map).ToArray());
                return false;
            }

            errors = default;
            return true;
        }

        private ValidationError Map(ValidationFailure arg)
        {
            return new ValidationError(arg.PropertyName, arg.ErrorCode, arg.ErrorMessage);
        }

        private void AddUserProfile(UserRegistered @event)
        {
            var userProfile = new UserProfile();
            var profileCommand = new CreateUserProfile(
                null,
                new UserProfileId(@event.Id),
                @event.DisplayName,
                @event.FullName,
                @event.EMail);
            userProfile.Handle(null, profileCommand);

            Session.StoreChanges(userProfile);
        }

        private async Task SendConfirmEmail(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _mailSender.SendConfirmEmail(_configuration, user.Id, user.DisplayName, user.Email, token);
        }
    }

    public class AddUserToRoleHandler : CommandHandler<AddUserToRole, Result>
    {
        private readonly UserManager<User> _userManager;

        public AddUserToRoleHandler(UserManager<User> userManager, IDocumentSession session) : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<Result> Execute(AddUserToRole command)
        {
            if (!command.InfrastructureCode || command.RequesterId != null)
            {
                var requester = await Session.LoadUserAsync(command.RequesterId);
                if (requester == null || !requester.IsAtLeast(Roles.ContentManager))
                {
                    return Result.Failed("RequesterId", ErrorCodes.UserNotFound, "not allowed.");
                }
            }

            var user = await _userManager.FindByIdAsync(command.UserToChangeId.ToString());
            if (user == null)
            {
                return Result.Failed("userId", ErrorCodes.UserNotFound, "User not found.");
            }

            var role = command.Role.GetName();
            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return new RegisterUserResult(result.ValidationErrors());
            }

            var @event = new UserToRoleAdded
            {
                EventTimestamp = DateTime.Now,
                RequesterId = command.RequesterId,
                UserToChangeId = command.UserToChangeId,
                Role = command.Role
            };

            Session.Events.Append(command.UserToChangeId.Value, @event);

            return Result.Success;
        }
    }

    public class RemoveUserFromRoleHandler : CommandHandler<RemoveUserFromRole, Result>
    {
        private readonly UserManager<User> _userManager;

        public RemoveUserFromRoleHandler(UserManager<User> userManager, IDocumentSession session) : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<Result> Execute(RemoveUserFromRole command)
        {
            var requester = await Session.LoadUserAsync(command.RequesterId);
            if (requester == null || !requester.IsAtLeast(Roles.ContentManager))
            {
                return Result.Failed("RequesterId", ErrorCodes.UserNotFound, "not allowed.");
            }

            var user = await _userManager.FindByIdAsync(command.UserToChangeId.ToString());
            if (user == null)
            {
                return Result.Failed("userId", ErrorCodes.UserNotFound, "User not found.");
            }

            var role = command.Role.GetName();
            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return new RegisterUserResult(result.ValidationErrors());
            }

            var @event = new UserFromRoleRemoved
            {
                EventTimestamp = DateTime.Now,
                RequesterId = command.RequesterId,
                UserToChangeId = command.UserToChangeId,
                Role = command.Role
            };

            Session.Events.Append(command.UserToChangeId.Value, @event);

            return Result.Success;
        }
    }

    public class ResetPasswordHandler : CommandHandler<ResetPassword>
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordHandler(UserManager<User> userManager, IDocumentSession session) : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<Result> Execute(ResetPassword command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user == null)
            {
                return Result.Failed("UserId", ErrorCodes.UserNotFound, "User not found");
            }

            if (command.NewPassword == null)
            {
                return Result.Failed("NewPassword", ErrorCodes.InvalidPassword, "Invalid password");
            }

            var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);

            return result.ToCommandResult();
        }
    }

    public class UserEMailPasswordLoginCommandHandler : QueryHandler<UserEMailPasswordLoginCommand, LoginResult>
    {
        private readonly UserManager<User> _userManager;

        public UserEMailPasswordLoginCommandHandler(UserManager<User> userManager, IDocumentSession session)
              : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<LoginResult> Execute(UserEMailPasswordLoginCommand query)
        {
            var user = await _userManager.FindByEmailAsync(query.EMail.Value);
            if (user == null)
            {
                return new LoginResult(false);
            }

            var valid = await _userManager.CheckPasswordAsync(user, query.Password.Value);

            return valid
                ? new LoginResult(true, user.Roles.ToArray(), new UserId(user.Id), new Name(user.DisplayName))
                : new LoginResult(false);
        }
    }

    public class ConfirmEmailCommandHandler : CommandHandler<ConfirmEmailCommand, Result>
    {
        private readonly UserManager<User> _userManager;

        public ConfirmEmailCommandHandler(UserManager<User> userManager, IDocumentSession session)
              : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<Result> Execute(ConfirmEmailCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user == null)
            {
                return Result.Failed("userId", ErrorCodes.UserNotFound, "User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, command.Token);
            if (!result.Succeeded)
            {
                return result.ToCommandResult();
            }

            var @event = new UserEmailConfirmed
            {
                EventTimestamp = DateTime.Now,
                UserId = command.UserId
            };

            Session.Events.Append(command.UserId.Value, @event);
            return result.ToCommandResult();
        }
    }

    public class RequestConfirmEmailCommandHandler : QueryHandler<RequestConfirmEmailCommand, Result>
    {
        private readonly IMailSender _mailSender;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public RequestConfirmEmailCommandHandler(UserManager<User> userManager, IDocumentSession session,
            IMailSender mailSender, IConfiguration configuration)
            : base(session)
        {
            _userManager = userManager;
            _mailSender = mailSender;
            _configuration = configuration;
        }

        protected override async Task<Result> Execute(RequestConfirmEmailCommand command)
        {
            var currentUser = await Session.LoadUserAsync(command.CurrentUserId);
            if (currentUser == null)
            {
                return Result.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Current User not found.");
            }

            if (!Equals(command.CurrentUserId, command.UserId) && !currentUser.IsAdministrator())
            {
                return Result.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Not allowed.");
            }

            var user = await _userManager.FindByIdAsync(command.UserId.ToString());
            if (user == null)
            {
                return Result.Failed("userId", ErrorCodes.UserNotFound, "User not found.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _mailSender.SendConfirmEmail(_configuration, (Guid)command.UserId, user.DisplayName, user.Email, token);

            return Result.Success;
        }
    }
    
    public class RequestAllConfirmEmailCommandHandler : QueryHandler<RequestAllConfirmEmailCommand, ContentResult>
    {
        private readonly IMailSender _mailSender;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public RequestAllConfirmEmailCommandHandler(UserManager<User> userManager, IDocumentSession session,
            IMailSender mailSender, IConfiguration configuration)
            : base(session)
        {
            _userManager = userManager;
            _mailSender = mailSender;
            _configuration = configuration;
        }

        protected override async Task<ContentResult> Execute(RequestAllConfirmEmailCommand command)
        {
            var currentUser = await Session.LoadUserAsync(command.CurrentUserId);
            if (currentUser == null)
            {
                return ContentResult.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Current User not found.");
            }

            if (!currentUser.IsAdministrator())
            {
                return ContentResult.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Not allowed.");
            }

            var users = await GetUnconfirmedUsers();
            var result = new StringBuilder();

            foreach (var user in users)
            {
                if (command.Filter != null &&
                    !string.Equals(user.Email, command.Filter, StringComparison.OrdinalIgnoreCase))
                {
                    result.AppendLine("skipped: " + user.Email);
                    continue;
                }

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _mailSender.SendConfirmEmail(_configuration, user.Id, user.DisplayName, user.Email, token);

                result.AppendLine("sent: " + user.Email);
            }

            return new ContentResult(true, result.ToString());
        }

        private async Task<IReadOnlyList<User>> GetUnconfirmedUsers()
        {
            return await Session.Query<User>()
                .Where(user => !user.EmailConfirmed)
                .ToListAsync();
        }
    }

    public class DisableUserHandler : CommandHandler<DisableUser>
    {
        private readonly UserManager<User> _userManager;

        public DisableUserHandler(UserManager<User> userManager, IDocumentSession session) : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<Result> Execute(DisableUser command)
        {
            var user = await _userManager.FindByEmailRequiredAsync(command.UserEMail);

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(100));

            return Result.Success;
        }
    }

    public class EnableUserHandler : CommandHandler<EnableUser>
    {
        private readonly UserManager<User> _userManager;

        public EnableUserHandler(UserManager<User> userManager, IDocumentSession session) : base(session)
        {
            _userManager = userManager;
        }

        protected override async Task<Result> Execute(EnableUser command)
        {
            var user = await _userManager.FindByEmailRequiredAsync(command.UserEMail);

            await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.SetLockoutEndDateAsync(user, null);

            return Result.Success;
        }
    }

    public class ExternalLoginHandler : CommandHandler<ExternalLoginCommand, LoginResult>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailSender _mailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalLoginHandler> _logger;
        private readonly IUserContactLogger _userContactLogger;

        public ExternalLoginHandler(UserManager<User> userManager, IDocumentSession session, ILogger<ExternalLoginHandler> logger, IMailSender mailSender, IConfiguration configuration, IUserContactLogger userContactLogger) : base(session)
        {
            _userManager = userManager;
            _logger = logger;
            _mailSender = mailSender;
            _configuration = configuration;
            _userContactLogger = userContactLogger;
        }

        protected override async Task<LoginResult> Execute(ExternalLoginCommand command)
        {
            var user = await GetOrRegisterUser(command);

            return user == null
                ? new LoginResult(false)
                : new LoginResult(true, user.Roles.ToArray(), new UserId(user.Id), new Name(user.DisplayName));
        }

        private async Task<User> GetOrRegisterUser(ExternalLoginCommand command)
        {
            var user = await GetByExternalId(command.ExternalType, command.ExternalIdentifier);
            if (user != null)
            {
                return user;
            }

            return GetByEmail(command.EMail) ?? await Register(command);
        }

        private async Task<User> GetByExternalId(string externalType, string externalIdentifier)
        {
            return await _userManager.FindByLoginAsync(externalType, externalIdentifier);
        }

        private User GetByEmail(EMail email)
        {
            var normalizedEmail = _userManager.NormalizeKey((string) email);

            var user = Session.Query<User>()
                .FirstOrDefault(criteria => criteria.NormalizedEmail == normalizedEmail);

            if (user != null)
            {
                _logger.LogInformation($"User external login: {email}");
            }

            return user;
        }

        private async Task<User> Register(ExternalLoginCommand command)
        {
            var user = new User((string) command.DisplayName, (string) command.EMail);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw result.ToBusinessException();
            }

            await _userManager.AddLoginAsync(user, new UserLoginInfo(command.ExternalIdentifier, command.ExternalType, (string) command.DisplayName));
            if (!result.Succeeded)
            {
                throw result.ToBusinessException();
            }

            var @event = new UserRegistered(
                new UserId(user.Id), 
                null, 
                Map(command.ExternalType),
                command.DisplayName,
                command.DisplayName,
                command.EMail,
                DateTime.Now
                );

            Session.Events.Append(user.Id, @event);

            AddUserProfile(@event);
            await SendConfirmEmail(user);

            return user;
        }

        private void AddUserProfile(UserRegistered @event)
        {
            var userProfile = new UserProfile();
            var profileCommand = new CreateUserProfile(
                null,
                new UserProfileId(@event.Id),
                @event.DisplayName,
                @event.DisplayName,
                @event.EMail);
            userProfile.Handle(null, profileCommand);

            Session.StoreChanges(userProfile);
        }

        private async Task SendConfirmEmail(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _mailSender.SendConfirmEmail(_configuration, user.Id, user.DisplayName, user.Email, token);

            _userContactLogger.Log(new UserId(user.Id), new EMail(user.Email), "ConfirmEmail", token);
        }

        private LoginType Map(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "facebook":
                    return LoginType.Facebook;

                case "google":
                    return LoginType.Google;

                default:
                    return LoginType.Unknown;
            }
        }
    }

    public class GenerateResetPasswordTokenHandler : CommandHandler<GenerateResetPasswordToken>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMailSender _emailSender;
        private readonly IConfiguration _configuration;

        public GenerateResetPasswordTokenHandler(UserManager<User> userManager, IDocumentSession session, IMailSender emailSender, IConfiguration configuration) : base(session)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task<Result> Execute(GenerateResetPasswordToken command)
        {
            var user = await _userManager.FindByEmailAsync(command.UserEMail);
            if (user == null) return Result.Success;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendResetPassword(_configuration, user.Id, user.DisplayName, user.Email, token);

            return Result.Success;
        }
    }
}