using System;
using Marten.Schema.Identity;

namespace PsychedelicExperience.Common
{
    /// <summary>
    /// Represents a globally unique identifier (GUID) with a
    /// shorter string value. Sguid
    /// Source: http://www.singular.co.nz/2007/12/shortguid-a-shorter-and-url-friendly-guid-in-c-sharp/
    /// </summary>
    public class ShortGuid
    {

        /// <summary>
        /// is guaranteed to be all zeroes.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);


        private Guid _guid;
        private string _value;

        public ShortGuid(string value)
        {
            _value = value;
            _guid = Decode(value);
        }

        public ShortGuid(Guid guid)
        {
            _value = Encode(guid);
            _guid = guid;
        }

        public Guid Guid
        {
            get { return _guid; }
            set
            {
                if (value != _guid)
                {
                    _guid = value;
                    _value = Encode(value);
                }
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    _guid = Decode(value);
                }
            }
        }

        public override string ToString()
        {
            return _value;
        }


        /// <summary>
        /// Returns a value indicating whether this instance and a
        /// specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var guid = obj as ShortGuid;
            if (guid != null)
                return _guid.Equals(guid._guid);
            if (obj is Guid)
                return _guid.Equals((Guid)obj);

            var stringValue = obj as string;
            if (stringValue != null)
                return _guid.Equals(new ShortGuid(stringValue)._guid);

            return false;
        }


        /// <summary>
        /// Returns the HashCode for underlying Guid.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        /// <summary>
        /// Initialises a new instance of the ShortGuid class
        /// </summary>
        /// <returns></returns>
        public static ShortGuid NewGuid()
        {
            return new ShortGuid(CombGuidIdGeneration.NewGuid());
        }


        /// <summary>
        /// Creates a new instance of a Guid using the string value,
        /// then returns the base64 encoded version of the Guid.
        /// </summary>
        /// <param name="value">An actual Guid string (i.e. not a ShortGuid)</param>
        /// <returns></returns>
        public static string Encode(string value)
        {
            Guid guid = new Guid(value);
            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given Guid as a base64 string that is 22
        /// characters long.
        /// </summary>
        /// <param name="guid">The Guid to encode</param>
        /// <returns></returns>
        public static string Encode(Guid guid)
        {
            string encoded = Convert.ToBase64String(guid.ToByteArray());
            encoded = encoded
                .Replace("/", "_")
                .Replace("+", "-");
            return encoded.Substring(0, 22);
        }

        /// <summary>
        /// Decodes the given base64 string
        /// </summary>
        /// <param name="value">The base64 encoded string of a Guid</param>
        /// <returns>A new Guid</returns>
        public static Guid Decode(string value)
        {
            value = value
                .Replace("_", "/")
                .Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(value + "==");
            return new Guid(buffer);
        }
       
        /// <summary>
        /// Determines if both ShortGuids have the same underlying
        /// Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(ShortGuid x, ShortGuid y)
        {
            if ((object)x == null) return (object)y == null;
            return (object)y != null && x._guid == y._guid;
        }

        /// <summary>
        /// Determines if both ShortGuids do not have the
        /// same underlying Guid value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(ShortGuid x, ShortGuid y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's string equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid._value;
        }

        /// <summary>
        /// Implicitly converts the ShortGuid to it's Guid equivilent
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid._guid;
        }

        /// <summary>
        /// Implicitly converts the string to a ShortGuid
        /// </summary>
        /// <param name="shortGuid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        /// <summary>
        /// Implicitly converts the Guid to a ShortGuid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static implicit operator ShortGuid(Guid guid)
        {
            return new ShortGuid(guid);
        }
    }

    public static class ShortGuidExtensions
    {
        public static string EncodeShortGuid(this Guid id)
        {
            return ShortGuid.Encode(id);
        }

        public static Guid DecodeShortGuid(this string shortGuid)
        {
            return ShortGuid.Decode(shortGuid);
        }
    }
}
