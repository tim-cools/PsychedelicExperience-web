using System;

namespace PsychedelicExperience.Psychedelics.Security
{
    public class ValidationRequestResult
    {
        private readonly bool _success;

        public static ValidationRequestResult UnknownError { get; } = new ValidationRequestResult("unknown-error");

        public Guid? OrganisationId { get; }
        public string Key { get;  }

        private ValidationRequestResult(string key, bool success = false, Guid? organisationId = null)
        {
            OrganisationId = organisationId;
            Key = key;
            _success = success;
        }

        public static ValidationRequestResult InvalidUser(Guid organisationId)
        {
            return new ValidationRequestResult("invalid-user", false, organisationId);
        }

        public static ValidationRequestResult RequestExpired(Guid organisationId)
        {
            return new ValidationRequestResult("request-expired", false, organisationId);
        }

        public static ValidationRequestResult UserNotLoggedin(Guid organisationId)
        {
            return new ValidationRequestResult("user-not-loggedin", false, organisationId);
        }

        public static ValidationRequestResult Success(Guid organisationId)
        {
            return new ValidationRequestResult("success", true, organisationId);
        }

        internal bool Success()
        {
            return _success;
        }

        protected bool Equals(ValidationRequestResult other)
        {
            return string.Equals(Key, other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValidationRequestResult) obj);
        }

        public override int GetHashCode()
        {
            return Key?.GetHashCode() ?? 0;
        }
    }
}