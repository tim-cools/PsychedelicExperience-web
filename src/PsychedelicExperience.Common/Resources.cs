using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PsychedelicExperience.Common
{
    public static class Resources
    {
        public static byte[] ReadResourceData(this Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));

            using (var stream = GetManifestResourceStream(type, name))
            using (var reader = new BinaryReader(stream))
            {                
                return reader.ReadBytes((int) stream.Length);
            }
        }

        public static string ReadResourceString(this Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));

            using (var stream = GetManifestResourceStream(type, name))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string[] ReadResourceLines(this Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));

            using (var stream = GetManifestResourceStream(type, name))
            using (var reader = new StreamReader(stream))
            {
                var result = new List<string>();
                var line = reader.ReadLine();
                while (line != null)
                {
                    result.Add(line);
                    line = reader.ReadLine();
                }
                return result.ToArray();
            }
        }

        public static IDictionary<string, string> ReadResourceDictionary(this Type type, string name)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (name == null) throw new ArgumentNullException(nameof(name));

            using (var stream = GetManifestResourceStream(type, name))
            using (var reader = new StreamReader(stream))
            {
                var result = new Dictionary<string, string>();
                var line = reader.ReadLine();
                while (line != null)
                {
                    var parts = line.Split(',');
                    if (parts.Length != 2)
                    {
                        throw new InvalidOperationException($"Invalid number of parts: {parts.Length} ({line}") ;
                    }

                    result.Add(parts[0].Trim(), parts[1].Trim());

                    line = reader.ReadLine();
                }
                return result;
            }
        }

        private static Stream GetManifestResourceStream(Type type, string name)
        {
            var fullName = $"{type.Namespace}.{name}";
            var manifestResourceStream = type.GetTypeInfo()
                .Assembly.GetManifestResourceStream(fullName);

            if (manifestResourceStream == null)
            {
                throw new InvalidOperationException($"Resource stream '{fullName}' not found. Did you set Build Action to Embedded Resource?");
            }
            return manifestResourceStream;
        }
    }
}