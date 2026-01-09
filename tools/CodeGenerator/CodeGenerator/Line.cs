using System;
using System.Linq;

namespace CodeGenerator
{
    internal class Line
    {
        public bool IsEmpty { get; }
        private readonly string[] _parts;

        public bool IsBlock { get; }

        private Line(bool isEmpty)
        {
            IsEmpty = isEmpty;
        }

        private Line(bool isBlock, string[] parts)
        {
            IsBlock = isBlock;

            _parts = parts;
        }

        public static Line Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return new Line(true);
            }

            var parts = line.Trim().Split(' ')
                .Select(part => part.Trim())
                .ToArray();

            var isBlock = !line.StartsWith("  ");
            return new Line(isBlock, parts);
        }

        public string Get(int index, string namespaceName)
        {
            if (_parts.Length < index + 1)
            {
                throw new InvalidOperationException(
                    $"Invalid number of parts: {_parts.Length}. Wanted {index + 1} for var {namespaceName})");
            }
            return _parts[index];
        }

        public string GetOptional(int index, string namespaceName)
        {
            return _parts.Length < index + 1 ? null : _parts[index];
        }
    }
}