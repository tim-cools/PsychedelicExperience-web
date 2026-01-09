
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace PsychedelicExperience.Common
{
    public static class StringExtensions
    {
        private static readonly char[] _allowedCharacter = {
            '@','!','#','$','%','&','\'',
            '*','+','-','/','=','?','.',
            '^','_','`','{','|','}','~'
        };

        public static string Generalize(this string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value.NormalizeForUrl();
        }

        public static string NormalizeForSearch(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var builder = new StringBuilder();
            var wasDash = true;
            foreach (var charValue in value)
            {
                var newValue = NormalizeForSearch(charValue);
                if (wasDash && newValue == '-') continue;

                builder.Append(newValue);
                wasDash = newValue == '-';
            }
            return builder.ToString().TrimEnd('-');
        }

        private static char NormalizeForSearch(char charValue)
        {
            return char.IsLetterOrDigit(charValue) || _allowedCharacter.Contains(charValue)
                ? char.ToLowerInvariant(charValue)
                : '-';
        }

        public static string NormalizeForUrl(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var urlValueBuilder = new StringBuilder();
            var wordBuilder = new StringBuilder();
            var newWord = true;

            foreach (var charValue in value)
            {
                var newValue = NormalizeForUrl(charValue);
                var valueIsSeparator = newValue == '-';

                if (newWord && valueIsSeparator) continue;

                if (valueIsSeparator)
                {
                    AddWord(wordBuilder, urlValueBuilder);

                    wordBuilder.Clear();
                    newWord = true;
                }
                else
                {
                    wordBuilder.Append(newValue);
                    newWord = false;
                }
            }

            AddWord(wordBuilder, urlValueBuilder);

            return urlValueBuilder.ToString();
        }

        private static void AddWord(StringBuilder wordBuilder, StringBuilder builder)
        {
            var wordValue = wordBuilder.ToString();
            if (string.IsNullOrEmpty(wordValue)) return;
            //if (!IsValidUrlWord(wordValue)) return;

            if (builder.Length > 0)
            {
                builder.Append("-");
            }

            builder.Append(wordValue);
        }

        private static bool IsValidUrlWord(string value) => !StopWords.Values.Contains(value);

        private static char NormalizeForUrl(char charValue)
        {
            return char.IsLetterOrDigit(charValue) 
                ? char.ToLowerInvariant(charValue)
                : '-';
        }

        public static string SmartTruncate(this string value, int length)
        {
            if (value == null || value.Length < length)
            {
                return value;
            }

            var nextSpace = value.LastIndexOf(" ", length, StringComparison.Ordinal);
            return $"{value.Substring(0, nextSpace > 0 ? nextSpace : length).Trim()}...";
        }
    }
}