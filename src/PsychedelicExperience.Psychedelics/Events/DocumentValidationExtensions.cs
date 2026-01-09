using FluentValidation;
using FluentValidation.Validators;

namespace PsychedelicExperience.Psychedelics.Events
{
    public static class EventValidationExtensions
    {
        //private class PropertyRatingPair : AbstractValidator<KeyValuePair<string, ScaleOf5>>
        //{
        //    public PropertyRatingPair()
        //    {
        //        RuleFor(value => value.Key).Property();
        //        RuleFor(value => value.Value).Rating();
        //    }
        //}

        private static readonly IPropertyValidator _lengthOf3to100 = new LengthValidator(3, 100);
        private static readonly IPropertyValidator _lengthOf10to5000 = new LengthValidator(10, 5000);
        
        //private static readonly IPropertyValidator _propertyValuesValidator = new SpecificValuesValidator(new []
        //{
        //    "accommodation",
        //    "Group size"
        //});

        private static readonly IPropertyValidator _notNull = new NotNullValidator();
        
        //private static readonly IPropertyValidator _sscaleOf5Enum = new EnumValidator(typeof(ScaleOf5));
        //private static readonly PropertyRatingPair _propertyPairValidator = new PropertyRatingPair();

        public static IRuleBuilderOptions<T, TProperty> EventId<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> EventType<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Privacy<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> UserId<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> MemberId<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }


        public static IRuleBuilderOptions<T, TProperty> DateTimeNotNull<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Location<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Image<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Status<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Content<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Reason<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> TagName<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Name<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_lengthOf3to100);
        }

        public static IRuleBuilderOptions<T, TProperty> Description<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_lengthOf10to5000);
        }
    }
}