using System;
using FluentValidation.Validators;

namespace PsychedelicExperience.Common
{
    public class IdHasValueValidator : PropertyValidator
    {
        public IdHasValueValidator() : base("Id has no value.")
        {
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var id = context.PropertyValue as Id;
            if (id == null) return false;

            return id.Value != Guid.Empty;
        }
    }
}