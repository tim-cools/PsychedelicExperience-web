using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.Clients
{
    public class ValidateClientResult : Result
    {
        public Guid Id { get; }
        public string Name { get; }
        public string AllowedOrigin { get; }
        public string RedirectUri { get;  }

        private ValidateClientResult(ValidationError [] errors)
            : base(false, errors)
        {
        }

        public ValidateClientResult(bool succeeded, Guid id = default(Guid), string name = null, string allowedOrigin = null, string redirectUri = null)
            : base(succeeded)
        {
            Id = id;
            Name = name;
            AllowedOrigin = allowedOrigin;
            RedirectUri = redirectUri;
        }

        public new static ValidateClientResult Failed(params ValidationError[] errors)
        {
            return new ValidateClientResult(errors);
        }
    }
}