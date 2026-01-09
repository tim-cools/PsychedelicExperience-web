using System.Text;

namespace CodeGenerator
{
    public static class StringExtensions
    {
        public static string PascalToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var builder = new StringBuilder();

            for (var index = 0; index < value.Length; index++)
            {
                var character = value[index];
                if (char.IsUpper(character))
                {
                    if (index != 0)
                    {
                        builder.Append('-');
                    }
                    builder.Append(char.ToLower(character));
                }
                else
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            if (value.Length == 1)
            {
                return new string(char.ToLowerInvariant(value[0]), 1);
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static string Plural(this string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 1) return value;

            if (value.Substring(value.Length - 1, 1).ToLower() == "y")
            {
                return value.Substring(0, value.Length - 1) + "ies";
            }

            if (value.Substring(value.Length - 1, 1).ToLower() != "s")
            {
                return value + "s";
            }

            return value;
        }
    }
}