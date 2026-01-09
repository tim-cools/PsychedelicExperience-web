using FluentValidation;
using FluentValidation.Validators;

namespace PsychedelicExperience.Membership.UserProfiles
{
    public static class ValidationExtensions
    {
        private static readonly IPropertyValidator _emailValidator = new EmailValidator();
        private static readonly IPropertyValidator _lengthOf3to100 = new LengthValidator(3, 100);
        private static readonly IPropertyValidator _lengthOf3to5000 = new LengthValidator(3, 5000);

        private static readonly IPropertyValidator _notNull = new NotNullValidator();

        public static IRuleBuilderOptions<T, TProperty> UserProfileId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> UserId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Name<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Description<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> TagLine<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Interval<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> File<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Email<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .SetValidator(_notNull)    ;
        }
    }
}