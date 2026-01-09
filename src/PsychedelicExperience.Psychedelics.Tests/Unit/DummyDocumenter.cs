using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PsychedelicExperience.Psychedelics.Tests.Unit
{
    internal class DummyDocumenter
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };

        private readonly StringBuilder _builder = new StringBuilder();
        private readonly JObject _value = new JObject();

        public void Header()
        {
        }

        public void Document<T>(T value)
        {
            var serializer = JsonSerializer.Create(_serializerSettings);

            _value[CamelCase(typeof(T).Name)] = JObject.FromObject(value, serializer);
        }

        public void DefaultExport()
        {
           var json = JsonConvert.SerializeObject(_value, _serializerSettings);

            WriteLine(json);
        }

        private void WriteLine(string text = null)
        {
            _builder.AppendLine(text);
        }

        private string CamelCase(string propertyName)
        {
            return propertyName.Substring(0, 1).ToLowerInvariant()
                 + propertyName.Substring(1);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
