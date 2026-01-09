using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PsychedelicExperience.Common
{
    public class CsvWriter
    {
        private string separator = ";";

        private IList<string> headers = new List<string>();
        private IDictionary<string, string> currentLine = new ConcurrentDictionary<string, string>();
        private IList<IDictionary<string, string>> lines = new List<IDictionary<string, string>>();

        private IList<string> headersOptional = new List<string>();
        private IDictionary<string, string> currentLineOptional = new ConcurrentDictionary<string, string>();
        private IList<IDictionary<string, string>> linesOptional = new List<IDictionary<string, string>>();

        public CsvWriter(string separator = ";")
        {
            this.separator = separator;
            lines.Add(currentLine);
            linesOptional.Add(currentLineOptional);
        }

        public void EndLine()
        {
            currentLine = new ConcurrentDictionary<string, string>();
            lines.Add(currentLine);

            currentLineOptional = new ConcurrentDictionary<string, string>();
            linesOptional.Add(currentLineOptional);
        }

        public void Write(string nameof, object value)
        {
            if (!headers.Contains(nameof))
            {
                headers.Add(nameof);
            }
            currentLine.Add(nameof, value?.ToString());
        }

        public void WriteSorted(string nameof, object value)
        {
            if (!headersOptional.Contains(nameof))
            {
                headersOptional.Add(nameof);
            }
            currentLineOptional.Add(nameof, value?.ToString());
        }

        public override string ToString()
        {
            var result = new StringWriter();

            var sortedOptionalHeaders = headersOptional.OrderBy(value => value);

            foreach (var header in headers)
            {
                AddValue(result, header);
            }
            foreach (var header in sortedOptionalHeaders)
            {
                AddValue(result, header);
            }

            result.WriteLine();

            for (var index = 0; index < lines.Count; index++)
            {
                AddValues(lines, index, headers, result);
                AddValues(linesOptional, index, sortedOptionalHeaders, result);

                result.WriteLine();
            }

            return result.ToString();
        }

        private void AddValues(IList<IDictionary<string, string>> list, int index, IEnumerable<string> headerValues, StringWriter result)
        {
            var line = list[index];

            foreach (var header in headerValues)
            {
                var value = line.ContainsKey(header) ? line[header] : string.Empty;
                AddValue(result, value);
            }
        }

        private void AddValue(StringWriter result, string value)
        {
            if (value != null && (value.Contains(" ") || value.Contains(@"""")))
            {
                value = value.Replace(@"""", @"""""");
                result.Write($@"""{value}""");
            }
            else
            {
                result.Write(value);
            }

            result.Write(separator);
        }
    }
}