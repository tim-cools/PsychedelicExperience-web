using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.Users
{
    public class EnsureAdministrator : IRequest<Result>
    {
        public UserId CurrentUserId { get; set; }

        public EnsureAdministrator(UserId currentUserId)
        {
            CurrentUserId = currentUserId;
        }
    }

    public class RegisterUserCommand : IRequest<RegisterUserResult>
    {
        public Name FullName { get; }
        public Name DisplayName { get; }
        public EMail EMail { get; }
        public Password Password { get; }
        public Password PasswordConfirm { get; set; }

        public RegisterUserCommand(Name fullName, Name displayName, EMail eMail, Password password, Password passwordConfirm)
        {
            FullName = fullName;
            DisplayName = displayName;
            EMail = eMail;
            Password = password;
            PasswordConfirm = passwordConfirm;
        }
    }

    public class RegisterUserResult : Result
    {
        public Guid Id { get; }

        public RegisterUserResult()
        {
        }

        public RegisterUserResult(Guid id) : base(true)
        {
            Id = id;
        }

        public RegisterUserResult(params ValidationError[] validationErrors) : base(false, validationErrors)
        {
        }
    }

    public class GenerateResetPasswordToken : IRequest<Result>
    {
        public string UserEMail { get; set; }
    }

    public class AddUserToRole : IRequest<Result>
    {
        public UserId RequesterId { get; set; }
        public UserId UserToChangeId { get; set; }
        public Role Role { get; set; }
        public bool InfrastructureCode { get; }

        public AddUserToRole(UserId requesterId, UserId userToChangeId, Role role, bool infrastructureCode = false)
        {
            RequesterId = requesterId;
            UserToChangeId = userToChangeId;
            Role = role;
            InfrastructureCode = infrastructureCode;
        }
    }

    public class RemoveUserFromRole : IRequest<Result>
    {
        public UserId RequesterId { get; set; }
        public UserId UserToChangeId { get; set; }
        public Role Role { get; set; }

        public RemoveUserFromRole(UserId requesterId, UserId userToChangeId, Role role)
        {
            RequesterId = requesterId;
            UserToChangeId = userToChangeId;
            Role = role;
        }
    }

    public enum Role
    {
        ContentManager,
        Administrator,
        ExperiencesBeta
    }

    public class ResetPassword : IRequest<Result>
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public UserId UserId { get; set; }
    }

    public class DisableUser : IRequest<Result>
    {
        public string UserEMail { get; set; }
    }

    public class EnableUser : IRequest<Result>
    {
        public string UserEMail { get; set; }
    }

    public class ExternalLoginCommand : IRequest<LoginResult>
    {
        public string ExternalType { get; }
        public Name DisplayName { get; }
        public string ExternalIdentifier { get; }
        public EMail EMail { get; }

        public ExternalLoginCommand(string externalType, Name displayrName, string externalIdentifier, EMail eMail)
        {
            ExternalType = externalType;
            DisplayName = displayrName;
            ExternalIdentifier = externalIdentifier;
            EMail = eMail;
        }
    }

    public class LoginResult : Result
    {
        public UserId UserId { get; }
        public Name DisplayName { get; }
        public string[] Roles { get; set; }

        public LoginResult()
        {
        }

        public LoginResult(bool success, string[] roles = null, UserId userId = null, Name displayName = null) : base(success)
        {
            Roles = roles;
            UserId = userId;
            DisplayName = displayName;
        }
    }

    public class UserEMailPasswordLoginCommand : IRequest<LoginResult>
    {
        public EMail EMail { get; }
        public Password Password { get; }

        public UserEMailPasswordLoginCommand(EMail email, Password password)
        {
            EMail = email;
            Password = password;
        }
    }

    public class ConfirmEmailCommand : IRequest<Result>
    {
        public UserId UserId { get; }
        public string Token { get; }

        public ConfirmEmailCommand(string token, UserId userId)
        {
            Token = token;
            UserId = userId;
        }
    }

    public class RequestConfirmEmailCommand : IRequest<Result>
    {
        public UserId CurrentUserId { get; }
        public UserId UserId { get; }

        public RequestConfirmEmailCommand(UserId currentUserId, UserId userId)
        {
            CurrentUserId = currentUserId;
            UserId = userId;
        }
    }

    public class RequestAllConfirmEmailCommand : IRequest<ContentResult>
    {
        public UserId CurrentUserId { get; }
        public string Filter { get; }

        public RequestAllConfirmEmailCommand(UserId currentUserId, string filter)
        {
            CurrentUserId = currentUserId;
            Filter = filter;
        }
    }
}