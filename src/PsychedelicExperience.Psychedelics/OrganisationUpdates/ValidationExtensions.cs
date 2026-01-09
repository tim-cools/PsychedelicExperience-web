using FluentValidation;
using FluentValidation.Validators;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdates
{
    public static class ValidationExtensions
    {
        private static readonly IPropertyValidator _idHasValue = new IdHasValueValidator();
        private static readonly IPropertyValidator _notNull = new NotNullValidator();
        private static readonly IPropertyValidator _lengthOf3To100 = new LengthValidator(3, 100);
        private static readonly IPropertyValidator _lengthOf10To5000 = new LengthValidator(10, 5000);
        private static readonly IPropertyValidator _organisationUpdatePrivacy = new EnumValidator(typeof(Messages.OrganisationUpdates.OrganisationUpdatePrivacy));

        public static IRuleBuilderOptions<T, TProperty> OrganisationId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> UserId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> OrganisationUpdateId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Subject<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .SetValidator(_notNull)
                .SetValidator(_lengthOf3To100);
        }

        public static IRuleBuilder<T, TProperty> Content<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .SetValidator(_notNull)
                .SetValidator(_lengthOf10To5000);
        }

        public static IRuleBuilder<T, TProperty> Privacy<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_organisationUpdatePrivacy);
        }
    }
}