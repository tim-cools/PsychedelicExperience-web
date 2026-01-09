using System.Threading.Tasks;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Membership.Users.Handlers;

namespace PsychedelicExperience.Membership.UserProfiles
{
    public class ChangeUserProfileEMailValidator : AbstractValidator<ChangeUserProfileEMail>
    {
        public ChangeUserProfileEMailValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.EMail).Email();
        }
    }

    public class ChangeUserProfileEMailHandler : AggregateCommandHandler<ChangeUserProfileEMail, UserProfile, UserProfileId>
    {
        private readonly UserManager<User> _userManager;

        public ChangeUserProfileEMailHandler(IDocumentSession session, IValidator<ChangeUserProfileEMail> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
            _userManager = userManager;
        }

        protected override async Task AdditionalCommand(Context<ChangeUserProfileEMail, UserProfile> context)
        {
            var id = context.Command.UserProfileId?.Value.ToString();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new BusinessException("User not found: " +  id);
            }

            var result = await _userManager.SetEmailAsync(user, context.Command.EMail.Value);
            if (!result.Succeeded)
            {
                throw result.ToBusinessException();
            }
        }
    }

    public class ChangeUserProfileDisplayNameValidator : AbstractValidator<ChangeUserProfileDisplayName>
    {
        public ChangeUserProfileDisplayNameValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.Name).Name();
        }
    }

    public class ChangeUserProfileDisplayNameHandler : AggregateCommandHandler<ChangeUserProfileDisplayName, UserProfile, UserProfileId>
    {
        private readonly UserManager<User> _userManager;

        public ChangeUserProfileDisplayNameHandler(IDocumentSession session, IValidator<ChangeUserProfileDisplayName> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
            _userManager = userManager;
        }

        protected override async Task AdditionalCommand(Context<ChangeUserProfileDisplayName, UserProfile> context)
        {
            var id = context.Command.UserProfileId?.Value.ToString();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new BusinessException("User not found: " + id);
            }

            user.DisplayName = (string) context.Command.Name;

            Session.Store(user);
        }
    }

    public class ChangeUserProfileFullNameValidator : AbstractValidator<ChangeUserProfileFullName>
    {
        public ChangeUserProfileFullNameValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.Name).Name();
        }
    }

    public class ChangeUserProfileFullNameHandler : AggregateCommandHandler<ChangeUserProfileFullName, UserProfile, UserProfileId>
    {
        public ChangeUserProfileFullNameHandler(IDocumentSession session, IValidator<ChangeUserProfileFullName> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeUserProfileDescriptionValidator : AbstractValidator<ChangeUserProfileDescription>
    {
        public ChangeUserProfileDescriptionValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.Description).Description();
        }
    }

    public class ChangeUserProfileDescriptionHandler : AggregateCommandHandler<ChangeUserProfileDescription, UserProfile, UserProfileId>
    {
        public ChangeUserProfileDescriptionHandler(IDocumentSession session, IValidator<ChangeUserProfileDescription> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeUserProfileTagLineValidator : AbstractValidator<ChangeUserProfileTagline>
    {
        public ChangeUserProfileTagLineValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.TagLine).TagLine();
        }
    }

    public class ChangeUserProfileTagLineHandler : AggregateCommandHandler<ChangeUserProfileTagline, UserProfile, UserProfileId>
    {
        public ChangeUserProfileTagLineHandler(IDocumentSession session, IValidator<ChangeUserProfileTagline> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeUserProfileAvatarValidator : AbstractValidator<ChangeUserProfileAvatar>
    {
        public ChangeUserProfileAvatarValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.File).File();
        }
    }

    public class ChangeUserProfileAvatarHandler : AggregateCommandHandler<ChangeUserProfileAvatar, UserProfile, UserProfileId>
    {
        public ChangeUserProfileAvatarHandler(IDocumentSession session, IValidator<ChangeUserProfileAvatar> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class DisableUserProfileNotificationEmailValidator : AbstractValidator<DisableUserProfileNotificationEmail>
    {
        public DisableUserProfileNotificationEmailValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
        }
    }

    public class DisableUserProfileNotificationEmailHandler : AggregateCommandHandler<DisableUserProfileNotificationEmail, UserProfile, UserProfileId>
    {
        public DisableUserProfileNotificationEmailHandler(IDocumentSession session, IValidator<DisableUserProfileNotificationEmail> commandValidator) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class EnableUserProfileNotificationEmailValidator : AbstractValidator<EnableUserProfileNotificationEmail>
    {
        public EnableUserProfileNotificationEmailValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
        }
    }

    public class EnableUserProfileNotificationEmailHandler : AggregateCommandHandler<EnableUserProfileNotificationEmail, UserProfile, UserProfileId>
    {
        public EnableUserProfileNotificationEmailHandler(IDocumentSession session, IValidator<EnableUserProfileNotificationEmail> commandValidator) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeUserProfileNotificationEmailIntervalValidator : AbstractValidator<ChangeUserProfileNotificationEmailInterval>
    {
        public ChangeUserProfileNotificationEmailIntervalValidator()
        {
            RuleFor(command => command.RequesterId).UserId();
            RuleFor(command => command.UserProfileId).UserProfileId();
            RuleFor(command => command.Interval).Interval();
        }
    }

    public class ChangeUserProfileNotificationEmailIntervalHandler : AggregateCommandHandler<ChangeUserProfileNotificationEmailInterval, UserProfile, UserProfileId>
    {
        public ChangeUserProfileNotificationEmailIntervalHandler(IDocumentSession session, IValidator<ChangeUserProfileNotificationEmailInterval> commandValidator, UserManager<User> userManager) :
            base(session, commandValidator,
                command => command.UserProfileId,
                command => command.RequesterId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }
}