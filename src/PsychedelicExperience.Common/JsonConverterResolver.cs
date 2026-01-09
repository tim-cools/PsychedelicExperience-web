using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace PsychedelicExperience.Common
{
    public static class JsonConverterResolver
    {
        public static IList<Type> Resolve<TModel, TMessages>()
        {
            return GetConverters<TModel>()
                .Union(GetIdConverters<TMessages>())
                .ToList();
        }

        public static IList<Type> Resolve<TModel>()
        {
            return GetConverters<TModel>()
                .ToList();
        }

        private static IEnumerable<Type> GetConverters<T>()
        {
            var assembly = typeof(T).GetTypeInfo().Assembly;
            return assembly.GetTypes()
                .Select(type => new {type, info = type.GetTypeInfo()})
                .Where(type => type.info.IsSubclassOf(typeof(JsonConverter)) && !type.info.IsAbstract && !type.info.IsGenericTypeDefinition)
                .Select(type => type.type);
        }
        
        private static IEnumerable<Type> GetIdConverters<T>()
        {
            var assembly = typeof(T).GetTypeInfo().Assembly;
            return assembly.GetTypes()
                .Select(type => new {type, info = type.GetTypeInfo()})
                .Where(type => type.info.IsSubclassOf(typeof(Id)) && !type.info.IsAbstract)
                .Select(type => typeof(IddConverter<>).MakeGenericType(type.type))
                .ToList();
        }
    }
}