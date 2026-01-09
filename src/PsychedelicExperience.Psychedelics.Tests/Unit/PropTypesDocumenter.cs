using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Baseline;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Unit
{
    internal class PropTypesDocumenter
    {
        private readonly StringBuilder _builder;
        private readonly IList<Type> _valueTypes = new List<Type>();
        private readonly IDictionary<Type, string> _typeMapping;

        private int _intent;

        public PropTypesDocumenter(StringBuilder builder)
        {
            _builder = builder;
            _typeMapping = new Dictionary<Type, string>
            {

                {typeof(bool?), "PropTypes.bool"},
                {typeof(bool), "PropTypes.bool.isRequired"},

                {typeof(int?), "PropTypes.number"},
                {typeof(decimal?), "PropTypes.number"},
                {typeof(double?), "PropTypes.number"},
                {typeof(long?), "PropTypes.number"},

                {typeof(int), "PropTypes.number.isRequired"},
                {typeof(decimal), "PropTypes.number.isRequired"},
                {typeof(double), "PropTypes.number.isRequired"},
                {typeof(long), "PropTypes.number.isRequired"},

                {typeof(string), "PropTypes.string"},
                {typeof(ShortGuid), "PropTypes.string"},

                {typeof(Guid), "PropTypes.string.isRequired"},
                {typeof(Guid?), "PropTypes.string"},
                {typeof(DateTime), "PropTypes.string.isRequired"},
                {typeof(DateTime?), "PropTypes.string"},

                {typeof(CenterStatus), "PropTypes.string"},
                {typeof(ScaleOf5), "PropTypes.string"},
                {typeof(WorkPrice), "PropTypes.string"},

                {typeof(Action), "PropTypes.func"},
            };
        }

        public void Header()
        {
            WriteLine("import {");
            WriteLine("  PropTypes");
            WriteLine("}");
            WriteLine("from '../vendors';");
            WriteLine();
        }

        public void Document<T>()
        {
            _valueTypes.Add(typeof(T));

            Scope($"var {CamelCase(typeof(T).Name)} = PropTypes.shape({{", () => Document(typeof(T)), Environment.NewLine + "});");

            WriteLine();
            WriteLine();
        }


        public void DefaultExport()
        {
            Scope("export default {", DocumentExport, "};");
        }

        private void DocumentExport()
        {
            WriteLine();

            for (var index = 0; index < _valueTypes.Count; index++)
            {
                var type = _valueTypes[index];
                WriteStart($"{CamelCase(type.Name)}");
                if (index != _valueTypes.Count - 1)
                {
                    Write(",");
                }
                WriteLine();
            }
        }

        private void Document(Type type)
        {
            WriteLine();

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            for (var index = 0; index < properties.Length; index++)
            {
                var property = properties[index];
                Document(property, index == properties.Length - 1);
            }
        }

        private void Document(PropertyInfo property, bool last)
        {
            WriteStart(CamelCase(property.Name) + ": ");
            PropType(property.PropertyType);

            if (!last)
            {
                WriteLine(",");
            }
        }

        private void PropType(Type propertyType)
        {
            if (_typeMapping.ContainsKey(propertyType))
            {
                Write(_typeMapping[propertyType]);
            }
            else if (propertyType.CanBeCastTo(typeof(IEnumerable)))
            {
                PrintArray(propertyType);
            }
            else
            {
                PrintComplexType(propertyType);
            }
        }

        private void PrintComplexType(Type propertyType)
        {
            Assert.False(propertyType.GetTypeInfo().IsValueType, propertyType.FullName);

            if (_valueTypes.Contains(propertyType))
            {
                Write(CamelCase(propertyType.Name));
            }
            else
            {
                Scope("PropTypes.shape({", () => Document(propertyType), "})");
            }
        }

        private void PrintArray(Type propertyType)
        {
            var types = GetEnumerableType(propertyType);
            switch (types.Length)
            {
                case 1:
                    Write("PropTypes.arrayOf(");
                    PropType(types[0]);
                    Write(")");
                    break;

                case 2: //dictionary
                    Write("PropTypes.any");
                    break;

                default:
                    throw new InvalidOperationException(propertyType.GenericTypeArguments.Length.ToString());
            }
        }

        private static Type[] GetEnumerableType(Type propertyType)
        {
            var elementType = propertyType.GetElementType();
            if (elementType != null) return new[] { elementType };

            return propertyType.GenericTypeArguments;
        }

        private void Scope(string begin, Action action, string end)
        {
            Write(begin);
            _intent++;

            action();

            _intent--;

            Write(end);
        }

        private void WriteStart(string text)
        {
            _builder.Append(new string(' ', _intent * 2) + text);
        }

        private void Write(string text)
        {
            _builder.Append(text);
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
    }
}
