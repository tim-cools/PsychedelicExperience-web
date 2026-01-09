using System;
using System.Reflection;
using Newtonsoft.Json;

namespace PsychedelicExperience.Common
{
    public abstract class CustomTypeConverter<T> : CustomTypeConverter where T : class
    {
        public override Type Type => typeof(T);
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueTyped = value as T;

            if (valueTyped != null)
            {
                writer.WriteValue(GetValue(valueTyped));
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
                var value = reader.Value.ToString();
                return Factory(value);
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

            var result = Factory(reader.Value?.ToString());
            if (!reader.Read())
            {
                throw new JsonSerializationException("Unexpected end when reading JSON.");
            }
            return result;
        }

        protected abstract string GetValue(T valueTyped);
        protected abstract T Factory(string value);

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }

    public class IddConverter<T> : CustomTypeConverter
    {
        private readonly ConstructorInfo _constructor;

        public override Type Type => typeof(T);

        public IddConverter()
        {
            _constructor = typeof(T).GetConstructor( new [] { typeof(Guid) });
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var id = value as Id;

            if (id != null)
            {
                writer.WriteValue(id.Value);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            var value = reader.Value.ToString();
            if (value.Length == ShortGuid.Empty.Value.Length)
            {
                return Factory(new ShortGuid(value));
            }
            Guid guid;
            if (!Guid.TryParse(value, out guid))
            {
                throw new InvalidOperationException("Could not serialize the Id");
            }
            return Factory(guid);
        }

        protected T Factory(Guid value)
        {
            return (T) _constructor.Invoke(new object[] { value });
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }
    }
}