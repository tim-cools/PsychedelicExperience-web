using System;
using System.Collections.Generic;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Surveys
{
    public class SurveyId : Id
    {
        public SurveyId(Guid value) : base(value)
        {
        }

        public static SurveyId New()
        {
            return new SurveyId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator SurveyId(Guid id)
        {
            return new SurveyId(id);
        }
    }

    public abstract class SurveyValue
    {
        public string Key { get; private set; }

        protected SurveyValue(string key)
        {
            Key = key;
        }
    }

    public class StringValue : SurveyValue
        {
            public string Value { get; private set; }
    
            public StringValue(string key, string value) : base(key) => Value = value;
        }
    
    public class DateTimeValue : SurveyValue
    {
        public DateTime Value { get; private set; }

        public DateTimeValue(string key, DateTime value) : base(key) => Value = value;
    }

        public class ArrayValue : SurveyValue
    {
        public string[] Values { get; private set; }

        public ArrayValue(string key, string[] value) : base(key) => Values = value;
    }

    public class SurveyData
    {
        public List<SurveyValue> Values { get; private set; } = new List<SurveyValue>();

        public void Add(string valueKey, object raw)
        {
            Values.Add(GetValue(valueKey, raw));
        }

        private static SurveyValue GetValue(string valueKey, object raw)
        {
            if (raw == null)
            {
                return new StringValue(valueKey, null);
            }

            var valueTypes = raw.GetType();
            if (valueTypes == typeof(string))
            {
                return new StringValue(valueKey, raw as string);
            }

            if (valueTypes == typeof(string[]))
            {
                return new ArrayValue(valueKey, raw as string[]);
            }

            throw new InvalidOperationException("Invalid values Type : " + valueTypes);
        }
    }
}