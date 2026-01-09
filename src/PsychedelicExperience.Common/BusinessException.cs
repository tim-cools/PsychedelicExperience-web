using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PsychedelicExperience.Common
{
    public class BusinessException  : Exception
    {
        public IEnumerable<ValidationError> Errors { get; }

        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
            Errors = new[] { new ValidationError(null, "system", message) };
        }

        public BusinessException(params ValidationError[] errors)
        {
            Errors = errors;
        }

        public BusinessException(string message, params ValidationError[] errors) : base(message)
        {
            Errors = errors;
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}