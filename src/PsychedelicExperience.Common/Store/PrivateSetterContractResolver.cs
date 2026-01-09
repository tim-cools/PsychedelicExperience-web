using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PsychedelicExperience.Common.Store
{
    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        private readonly IDictionary<Type, JsonConverter> _customConverters;

        public PrivateSetterContractResolver(IDictionary<Type, JsonConverter> customConverters)
        {
            if (customConverters == null) throw new ArgumentNullException(nameof(customConverters));

            _customConverters = customConverters;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);
            JsonConverter converter;
            if (_customConverters.TryGetValue(objectType, out converter))
            {
                contract.Converter = converter;
            }
            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }

    public class PrivateSetterCamelCasePropertyNamesContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jProperty = base.CreateProperty(member, memberSerialization);
            if (jProperty.Writable)
                return jProperty;

            jProperty.Writable = member.IsPropertyWithSetter();

            return jProperty;
        }
    }

    internal static class MemberInfoExtensions
    {
        internal static bool IsPropertyWithSetter(this MemberInfo member)
        {
            var property = member as PropertyInfo;

            return property?.GetSetMethod(true) != null;
        }
    }
}