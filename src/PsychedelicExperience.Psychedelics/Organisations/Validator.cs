//using System.Collections.Generic;
//using System.Linq;
//using Baseline;
//using FluentValidation;
//using FluentValidation.Internal;
//using FluentValidation.Validators;
//using PsychedelicExperience.Common;
//using PsychedelicExperience.Membership.Users.Domain;
//using PsychedelicExperience.Psychedelics.Messages.Organisations;

//namespace PsychedelicExperience.Psychedelics.Organisations
//{
//    internal class Validator
//    {
//        private readonly IList<string> _messages = new List<string>();
//        private readonly ValidationContext _context;

//        private static readonly IPropertyValidator _notNullValidator = new NotNullValidator();

//        public Validator(Organisation command)
//        {
//            _context = new ValidationContext(command);
//        }

//        public Validator User(User user)
//        {
//            Validate(user, _notNullValidator);
//            return this;
//        }

//        public Validator Id(OrganisationId organisationId)
//        {
//            Validate(organisationId, _notNullValidator);
//            return this;
//        }

//        private static InlineValidator<Organisation> _nameValidator = new InlineValidator<Organisation>()
//            .RuleFor()
//            .With(MaximumLengthValidator(20, () => "Name should be maximum 20 characters."));

//        private static readonly IPropertyValidator _nameMaxLength = new;

//        public Validator Name(string name)
//        {
//            Validate(name, _nameMinLength);
//            Validate(name, _nameMaxLength);
//            return this;
//        }

//        private static readonly IPropertyValidator _descriptionMinLength = new MinimumLengthValidator(20, () => "Description should be at least 20 characters.");
//        private static readonly IPropertyValidator _descriptionMaxLength = new MaximumLengthValidator(10000, () => "Description should be maximum 10000 characters.");

//        public Validator Description(string description)
//        {
//            Validate(description, _descriptionMinLength);
//            Validate(description, _descriptionMaxLength);

//            return this;
//        }

//        private bool Validate(object value, IPropertyValidator validator)
//        {
//            var result = validator.Validate(context).ToList();

//            if (result.Any())
//            {
//                _messages.AddRange(result.Select(message => message.ErrorMessage));
//                return false;
//            }
//            return true;
//        }

//        private PropertyValidatorContext Context(object value, PropertyRule rule, string name)
//        {
//            return new PropertyValidatorContext(_context, rule, name, value);
//        }

//        public void Verify()
//        {
//            if (_messages.Any())
//            {
//                throw new BusinessException(_messages);
//            }
//        }
//    }

//    public static class InlineValidatorExtensions
//    {
//        public static InlineValidator<T> With<T>(this InlineValidator<T> inlineValidator, IPropertyValidator validator)
//        {
//            inlineValidator.RuleFor();
//            return inlineValidator;
//        }
//    }
//}