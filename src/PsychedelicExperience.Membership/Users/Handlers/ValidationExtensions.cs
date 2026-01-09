using System;
using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Resources;
using FluentValidation.Validators;
using PsychedelicExperience.Membership.Messages;

namespace PsychedelicExperience.Membership.Users
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, Name> Length<T>(
            this IRuleBuilder<T, Name> ruleBuilder,
            int min,
            int max)
        {
            return ruleBuilder.SetValidator(new LengthValidator(min, max));
        }

        public static IRuleBuilderOptions<T, EMail> Length<T>(
            this IRuleBuilder<T, EMail> ruleBuilder,
            int min,
            int max)
        {
            return ruleBuilder.SetValidator(new LengthValidator(min, max));
        }

        public static IRuleBuilderOptions<T, Name> Matches<T>(
            this IRuleBuilder<T, Name> ruleBuilder,
            string expression)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
        }

        public static IRuleBuilderOptions<T, EMail> Matches<T>(
            this IRuleBuilder<T, EMail> ruleBuilder,
            string expression)
        {
            return ruleBuilder.SetValidator(new RegularExpressionValidator(expression));
        }
    }

    public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator, IPropertyValidator
    {
        private readonly Func<object, Regex> _regexFunc;

        public RegularExpressionValidator(string expression)
            : base((IStringSource) new LanguageStringSource(nameof(RegularExpressionValidator)))
        {
            this.Expression = expression;
            Regex regex = RegularExpressionValidator.CreateRegex(expression, RegexOptions.None);
            this._regexFunc = (Func<object, Regex>) (x => regex);
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            Regex regex = this._regexFunc(context.Instance);
            if (regex == null || context.PropertyValue == null || regex.IsMatch(context.PropertyValue.ToString()))
                return true;
            context.MessageFormatter.AppendArgument("RegularExpression", (object) regex.ToString());
            return false;
        }

        private static Regex CreateRegex(string expression, RegexOptions options = RegexOptions.None)
        {
            try
            {
                if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null)
                    return new Regex(expression, options, TimeSpan.FromSeconds(2.0));
            }
            catch
            {
            }

            return new Regex(expression, options);
        }

        public string Expression { get; }
    }
}