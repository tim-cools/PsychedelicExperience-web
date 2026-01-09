using System;
using System.Runtime.CompilerServices;
using Marten.Schema.Identity;

namespace PsychedelicExperience.Common
{
    public abstract class Id : IComparable
    {
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public Guid Value { get; }

        protected Id(Guid value)
        {
            Value = value;
        }

        public static explicit operator Guid(Id id)
        {
            if (id == null)
            {
                throw new InvalidCastException("Id is null");
            }
            return id.Value;
        }

        public static explicit operator ShortGuid(Id id)
        {
            if (id == null)
            {
                throw new InvalidCastException("Id is null");
            }
            return id.Value;
        }

        public static explicit operator Guid? (Id id)
        {
            return id?.Value;
        }

        public static explicit operator string (Id id)
        {
            return id?.Value.ToString("N");
        }

        public int CompareTo(object obj)
        {
            var other = obj as Id;
            return other == null ? -1 : Value.CompareTo(other.Value);
        }
        
        public override string ToString()
        {
            return $"{Value}";
        }
      
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return ((Id) obj).Value == Value;
        }

        public DateTimeOffset GetTimestamp()
        {
            try
            {
                return CombGuidIdGeneration.GetTimestamp(Value);
            }
            catch (ArgumentException)
            {
                return DateTimeOffset.MinValue;  //legacy data, we ignore this...
            }
        }
    }

    public class OptionalDescription
    {
        public bool Available { get; set; }
        public string Description { get; set; }

        public OptionalDescription()
        {
        }

        public OptionalDescription(string description)
        {
            Available = true;
            Description = description;
        }
    }
}