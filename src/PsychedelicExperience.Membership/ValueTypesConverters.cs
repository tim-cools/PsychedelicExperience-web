using System;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;

namespace PsychedelicExperience.Membership.Messages
{
    public class EMailConvertor : CustomTypeConverter<EMail>
    {
        protected override string GetValue(EMail valueTyped)
        {
            return valueTyped.Value;
        }

        protected override EMail Factory(string value)
        {
            return new EMail(value);
        }
    }

    public class PasswordConvertor : CustomTypeConverter<Password>
    {
        protected override string GetValue(Password valueTyped)
        {
            return valueTyped.Value;
        }

        protected override Password Factory(string value)
        {
            return new Password(value);
        }
    }

    public class NameConvertor : CustomTypeConverter<Name>
    {
        protected override string GetValue(Name valueTyped)
        {
            return valueTyped.Value;
        }

        protected override Name Factory(string value)
        {
            return new Name(value);
        }
    }

    public class TitleConvertor : CustomTypeConverter<Title>
    {
        protected override string GetValue(Title valueTyped)
        {
            return valueTyped.Value;
        }

        protected override Title Factory(string value)
        {
            return new Title(value);
        }
    }

    public class DescriptionConvertor : CustomTypeConverter<Description>
    {
        protected override string GetValue(Description valueTyped)
        {
            return valueTyped.Value;
        }

        protected override Description Factory(string value)
        {
            return new Description(value);
        }
    }

    public class EncryptedStringConverter : CustomTypeConverter
    {
        public override Type Type => typeof(EncryptedString);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueTyped = value as EncryptedString;

            if (valueTyped?.Value != null)
            {
                writer.WriteValue(valueTyped.Value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return Factory(reader);
            }

            if (!reader.Read())
            {
                throw new JsonSerializationException("Unexpected end when reading JSON.");
            }
            if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != "Value")
            {
                throw new JsonSerializationException($"Expected token type: 'PropertyName' and value 'Value' (is: {reader.TokenType} and {reader.Value.ToString()})");
            }

            if (!reader.Read())
            {
                throw new JsonSerializationException("Unexpected end when reading JSON.");
            }
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonSerializationException($"Expected token type: 'string' (is: {reader.TokenType})");
            }
        
            var result = Factory(reader);
            if (!reader.Read())
            {
                throw new JsonSerializationException("Unexpected end when reading JSON.");
            }
            return result;
        }

        private EncryptedString Factory(JsonReader reader)
        {
            var value = reader.Value?.ToString();
            var data = ConvertBytes(value);

            return data != null ? new EncryptedString(data) : null;
        }

        private byte[] ConvertBytes(string value)
        {
            try
            {
                return value != null ? Convert.FromBase64String(value) : null;
            }
            catch (FormatException)
            {
                return null; //we ignore invalid base64 strings in the database, this is legacy data
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EncryptedString);
        }
    }
}