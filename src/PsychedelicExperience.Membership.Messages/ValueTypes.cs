using System;
using System.Collections.Generic;

namespace PsychedelicExperience.Membership.Messages
{
    public class EMail : IComparable
    {
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string Value { get; }

        public EMail(string value)
        {
            Value = value;
        }

        public static explicit operator string(EMail value)
        {
            return value?.Value;
        }

        public static explicit operator EMail(string value)
        {
            return new EMail(value);
        }

        public int CompareTo(object obj)
        {
            var other = obj as EMail;
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

            return ((EMail)obj).Value == Value;
        }

        public EMail Normalize()
        {
            return new EMail(Value.ToLowerInvariant());
        }
    }

    public class Password : IComparable
    {
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string Value { get; }

        public Password(string value)
        {
            Value = value;
        }

        public static explicit operator string(Password id)
        {
            return id?.Value;
        }

        public static explicit operator Password(string value)
        {
            return new Password(value);
        }

        public int CompareTo(object obj)
        {
            var other = obj as Password;
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

            return ((Password)obj).Value == Value;
        }
    }

    public class Name : IComparable
    {
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string Value { get; }

        public Name(string value)
        {
            Value = value;
        }

        public static explicit operator string(Name name)
        {
            return name?.Value;
        }

        public static explicit operator Name(string value)
        {
            return new Name(value);
        }

        public int CompareTo(object obj)
        {
            var other = obj as Name;
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

            return ((Name)obj).Value == Value;
        }
    }

    public class Title : IComparable
    {
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string Value { get; }

        public Title(string value)
        {
            Value = value;
        }

        public static explicit operator string(Title id)
        {
            return id?.Value;
        }

        public static explicit operator Title(string value)
        {
            return new Title(value);
        }

        public int CompareTo(object obj)
        {
            var other = obj as Title;
            return other == null ? -1 : Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return ((Title)obj).Value == Value;
        }
    }

    public class Description : IComparable
    {
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public string Value { get; }

        public Description(string value)
        {
            Value = value;
        }

        public static explicit operator string(Description value)
        {
            return value?.Value;
        }

        public static explicit operator Description(string value)
        {
            return new Description(value);
        }

        public int CompareTo(object obj)
        {
            var other = obj as Description;
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

            return ((Description)obj).Value == Value;
        }
    }

    public class Address
    {
        public string Name { get; }
        public string Country { get; set; }
        public Location Location { get; set; }
        public IDictionary<string, string> Attributes { get; }
        public string PlaceId { get; set; }

        public Address(string name, string country, Location location, string placeId, IDictionary<string, string> attributes)
        {
            Name = name;
            Country = country;
            Location = location;
            PlaceId = placeId;
            Attributes = attributes;
        }
    }

    public class Location
    {
        public decimal Latitude { get; }
        public decimal Longitude { get; }

        public Location(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}