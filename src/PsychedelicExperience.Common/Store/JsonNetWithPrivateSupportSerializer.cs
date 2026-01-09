using System;
using System.Collections.Generic;
using System.IO;
using Marten;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PsychedelicExperience.Common.Store
{
    public class JsonNetWithPrivateSupportSerializer : ISerializer
    {
        private readonly JsonSerializer _clean;
        private readonly JsonSerializer _serializer;

        public JsonNetWithPrivateSupportSerializer(IDictionary<Type, JsonConverter> customConverters)
        {
            _clean = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.None,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new PrivateSetterContractResolver(customConverters)
            };

            _serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.Auto,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
                ContractResolver = new PrivateSetterContractResolver(customConverters)
            };

            var converter = new StringEnumConverter();
            _serializer.Converters.Add(converter);
            _clean.Converters.Add(converter);
        }

        public string ToJson(object document)
        {
            var writer = new StringWriter();
            _serializer.Serialize(writer, document);

            return writer.ToString();
        }

        public void ToJson(object document, TextWriter writer)
        {
            _serializer.Serialize(writer, document);
        }

        public T FromJson<T>(string json)
        {
            return _serializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));
        }

        public object FromJson(Type type, string json)
        {
            return _serializer.Deserialize(new JsonTextReader(new StringReader(json)), type);
        }

        public T FromJson<T>(Stream stream)
        {
            return _serializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
        }

        public T FromJson<T>(TextReader reader)
        {
            return _serializer.Deserialize<T>(new JsonTextReader(reader));
        }

        public object FromJson(Type type, TextReader reader)
        {
            return _serializer.Deserialize(new JsonTextReader(reader), type);
        }

        public string ToCleanJson(object document)
        {
            var writer = new StringWriter();
            _clean.Serialize(writer, document);

            return writer.ToString();
        }

        public EnumStorage EnumStorage => EnumStorage.AsString;
        public Casing Casing => Casing.Default;
        public CollectionStorage CollectionStorage => CollectionStorage.Default;
        public NonPublicMembersStorage NonPublicMembersStorage => NonPublicMembersStorage.Default;
    }
}