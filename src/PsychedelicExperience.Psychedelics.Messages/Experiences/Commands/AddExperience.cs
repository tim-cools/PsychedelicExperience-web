using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Commands
{
    public abstract class ExperienceValue
    {
        public abstract object Raw();
    }

    public class StringValue : ExperienceValue
    {
        public string Value { get; private set; }

        public StringValue(string value) => Value = value;

        public override string ToString() => Value;

        public override object Raw() => Value;
    }

    public class ArrayValue : ExperienceValue
    {
        public List<string> Values { get; private set; } = new List<string>();

        public ArrayValue(string value) => Values.Add(value);

        public void AddValue(string value) => Values.Add(value);

        public override string ToString() => string.Join(",", Values.ToArray());

        public override object Raw() => Values.ToArray();
    }

    public class ExperienceData : IEnumerable<string>
    {
        private readonly IDictionary<string, ExperienceValue> _values = new Dictionary<string, ExperienceValue>();

        public string PopString(string key)
        {
            if (!_values.TryGetValue(key, out var value)) return null;

            _values.Remove(key);
            return value.ToString();
        }

        public object PopRaw(string key)
        {
            if (!_values.TryGetValue(key, out var value)) return null;

            _values.Remove(key);
            return value.Raw();
        }

        public string[] PopArray(string key)
        {
            if (!_values.TryGetValue(key, out var value))
            {
                return null;
            }

            _values.Remove(key);
            var arrayValue = value as ArrayValue;
            if (arrayValue == null)
            {
                throw new InvalidOperationException($"Value {key} is not an array ({value.GetType()})");
            }

            return arrayValue.Values.ToArray();
        }
        public void AddValue(string key, string value)
        {
            _values.Add(key, new StringValue(value));
        }

        public void AddArray(string key, string value)
        {
            if (_values.TryGetValue(key, out var arrayValue) )
            {
                ((ArrayValue) arrayValue).AddValue(value);
            }
            else
            {
                _values.Add(key, new ArrayValue(value));
            }
        }

        public IEnumerator<string> GetEnumerator() => _values.Keys.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class AddExperience : IRequest<Result>
    {
        public UserId UserId { get; }
        public ExperienceId ExperienceId { get; }
        public ExperienceData Data { get; }

        public AddExperience(UserId userId, ExperienceId experienceId, ExperienceData data)
        {
            UserId = userId;
            ExperienceId = experienceId;
            Data = data;
        }
    }
}
