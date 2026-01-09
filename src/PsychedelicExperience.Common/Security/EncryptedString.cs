using System;

namespace PsychedelicExperience.Common.Security
{
    public class EncryptedString
    {
        public string Value { get; set; }

        public EncryptedString()
        {
        }

        public EncryptedString(byte[] value)
        {
            Value = Convert.ToBase64String(value);
        }

        public EncryptedString(string value)
        {
            Value = value;
        }

        public byte[] GetBytes()
        {
            return Convert.FromBase64String(Value);
        }

        protected bool Equals(EncryptedString other)
        {
            return Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EncryptedString) obj);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}