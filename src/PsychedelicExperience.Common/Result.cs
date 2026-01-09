using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsychedelicExperience.Common
{
    public class Result
    {
        private static readonly Result _success = new Result(true);
        public static Result Success { get; } = _success;

        public bool Succeeded { get; internal set; }
        public IEnumerable<ValidationError> Errors { get; internal set; }

        public Result()
        {
        }

        protected Result(bool success, params ValidationError[] errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            Succeeded = success;
            Errors = errors;
        }

        public static Result Failed(string property, string code, string description)
        {
            return new Result(false, new ValidationError(property, code, description));
        }

        public static Result Failed(params ValidationError[] errors)
        {
            return new Result(false, errors);
        }

        public Result Merge(Result second)
        {
            if (second == null) throw new ArgumentNullException(nameof(second));

            if (second.Succeeded && Succeeded)
            {
                return Success;
            }

            var combinedErrors = Errors.Union(second.Errors).ToArray();
            return Failed(combinedErrors);
        }

        public override string ToString()
        {
            var errors = new StringBuilder();
            if (Errors != null)
            {
                foreach (var error in Errors)
                {
                    errors.AppendLine(" > " + error);
                }
            }
            return (Succeeded ? "Succeeded" : "Failed:") + errors;
        }
    }

    public class ValidationError
    {
        public string Property { get; }
        public string Code { get; }
        public string Message { get; }

        public ValidationError(string property, string code, string message)
        {
            Property = property;
            Code = code;
            Message = message;
        }

        public override string ToString()
        {
            return $"{nameof(Property)}: {Property}, {nameof(Code)}: {Code}, {nameof(Message)}: {Message}";
        }

        protected bool Equals(ValidationError other)
        {
            return string.Equals(Property, other.Property) && string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValidationError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Property != null ? Property.GetHashCode() : 0)*397) ^ (Code != null ? Code.GetHashCode() : 0);
            }
        }
    }

    public class ContentResult : Result
    {
        public string Content { get; set; }

        public ContentResult()
        {
        }

        public ContentResult(string errorcode, string content = null) : base(false, new ValidationError(null, errorcode, null))
        {
            Content = content;

        }

        public ContentResult(bool success, string content = null) : base(success)
        {
            Content = content;
        }

        public ContentResult(bool success, ValidationError error) : base(success, error)
        {
        }

        public new static ContentResult Failed(string property, string code, string description)
        {
            return new ContentResult(false, new ValidationError(property, code, description));
        }
    }

}