using System;
using Newtonsoft.Json;

namespace PsychedelicExperience.Common
{
    public class ShortGuidConverter : CustomTypeConverter
    {
        public override Type Type => typeof(ShortGuid);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var id = value as ShortGuid;

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
                return new ShortGuid(value);
            }
            Guid guid;
            if (!Guid.TryParse(value, out guid))
            {
                throw new InvalidOperationException("Could not serialize the Id");
            }
            return new ShortGuid(guid);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ShortGuid);
        }
    }
}