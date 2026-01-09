using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using System.Linq;
using FluentValidation.Resources;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Psychedelics.Tags.Handlers;

namespace PsychedelicExperience.Psychedelics.Organisations
{

    public class CenterReviewValidator : AbstractValidator<CenterReview>
    {
        public CenterReviewValidator()
        {
            RuleFor(review => review.Location).NotNull();
            RuleFor(review => review.Accommodation).NotNull();
            RuleFor(review => review.Facilitators).NotNull();
            RuleFor(review => review.Medicine).NotNull();
        }
    }

    public class SpecificValuesValidator : PropertyValidator, IStringSource
    {
        private readonly string[] _values;

        public SpecificValuesValidator(string[] values)
          : base(string.Empty)
        {
            Options.ErrorMessageSource = this;
            _values = values;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return context.PropertyValue == null
                || _values.Contains(context.PropertyValue);
        }

        public string GetString(IValidationContext context)
        {
            return $"Invalid value: {context.PropertyValue} (Should be {string.Join(',', _values)})";
        }

        public string ResourceName { get; }
        public Type ResourceType { get; }
    }

    public static class ValidationExtensions
    {
        private class PropertyRatingPair : AbstractValidator<KeyValuePair<string, ScaleOf5>>
        {
            public PropertyRatingPair()
            {
                RuleFor(value => value.Key).Property();
                RuleFor(value => value.Value).Rating();
            }
        }

        private static readonly IPropertyValidator _lengthOf3To250 = new LengthValidator(3, 250);
        private static readonly IPropertyValidator _lengthOf10To100k = new LengthValidator(10, 100000);

        private static readonly IPropertyValidator _propertyValuesValidator = new SpecificValuesValidator(new []
        {
            "accommodation",
            "Group size"
        });

        private static readonly IPropertyValidator _notNull = new NotNullValidator();
        private static readonly IPropertyValidator _sscaleOf5Enum = new EnumValidator(typeof(ScaleOf5));
        private static readonly IPropertyValidator _typeValidator = new SpecificValuesValidator(TagRepository.GetTags(TagsDomain.OrganisationTypes, true)
            .Select(value => value.Name).ToArray());

        public static IRuleBuilderOptions<T, TProperty> OrganisationId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> UserId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> OrganisationReviewId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> PhotoFiles<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> PhotoId<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Name<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .SetValidator(_notNull)
                .SetValidator(_lengthOf3To250);
        }

        public static IRuleBuilder<T, TProperty> Visited<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Description<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull)
                .SetValidator(_lengthOf10To100k);
        }

        public static IRuleBuilder<T, TProperty> Tags<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Address<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Center<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : Center
        {
            return ruleBuilder; //.SetValidator(new CenterReviewValidator());
        }

        public static IRuleBuilder<T, TProperty> Practitioner<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : Practitioner
        {
            return ruleBuilder; //.SetValidator(new CenterReviewValidator());
        }

        public static IRuleBuilder<T, TProperty> CenterReview<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : CenterReview
        {
            return ruleBuilder.SetValidator(new CenterReviewValidator());
        }

        public static IRuleBuilder<T, TProperty> Community<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : Community
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> CommunityReview<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : CommunityReview
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> HealthcareProvider<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : HealthcareProvider
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> HealthcareProviderReview<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder) where TProperty : HealthcareProviderReview
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Rating<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_sscaleOf5Enum);
        }

        public static IRuleBuilder<T, TProperty> Property<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_propertyValuesValidator);
        }

        public static IRuleBuilder<T, TProperty> Website<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Phone<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> Email<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> UserIdOptional<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder; //.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> TagName<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Type<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder
                .SetValidator(_notNull)
                .SetValidator(_typeValidator);
        }

        public static IRuleBuilderOptions<T, TProperty> Relation<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> Reason<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilderOptions<T, TProperty> InfoTitle<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(_notNull);
        }

        public static IRuleBuilder<T, TProperty> InfoContent<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder;
        }
    }
}