using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Users.Handlers
{
    public static class IdentityResultExtensions
    {
        private static readonly IDictionary<string, string> ErrorMapping = new Dictionary<string, string>
        {
            { "PasswordRequiresNonAlphanumeric", @"Passwords must have at least one non alphanumeric character. Eg: _ - & ( )" },
            { "PasswordRequiresLower",           @"Passwords must have at least one lowercase ('a'-'z')." }
        };

        private static readonly IDictionary<string, string> ErrorPropertyMapping = new Dictionary<string, string>
        {
            { "InvalidUserName", "DisplayName" },
            { "DuplicateUserName", "DisplayName" },
            { "InvalidEmail", "EMail" },
            { "DuplicateEmail", "EMail" },
            { "PasswordMismatch", "Password" },
            { "PasswordTooShort", "Password" },
            { "PasswordRequiresUniqueChars", "Password" },
            { "PasswordRequiresNonAlphanumeric", "Password" },
            { "PasswordRequiresDigit", "Password" },
            { "PasswordRequiresLower", "Password" },
            { "PasswordRequiresUpper", "Password" },
        };

        public static Result ToCommandResult(this IdentityResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return result.Succeeded
                ? Result.Success
                : Result.Failed(result.ValidationErrors());
        }

        public static BusinessException ToBusinessException(this IdentityResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return new BusinessException(result.ValidationErrors());
        }

        public static ValidationError[] ValidationErrors(this IdentityResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            return result.Errors.Select(Map).ToArray();
        }

        private static ValidationError Map(IdentityError arg)
        {
            var description = Description(arg);
            var property = Property(arg);

            return new ValidationError(property, arg.Code, description);
        }

        private static string Property(IdentityError arg)
            => !ErrorPropertyMapping.TryGetValue(arg.Code, out var value) ? string.Empty : value;

        private static string Description(IdentityError arg)
            => !ErrorMapping.TryGetValue(arg.Code, out var value) ? arg.Description : value;

        public static void ThrowWhenFailed(this IdentityResult result, string message)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));

            if (!result.Succeeded)
            {
                throw new BusinessException(message, result.ValidationErrors());
            }
        }
    }
}