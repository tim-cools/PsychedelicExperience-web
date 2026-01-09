using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Psychedelics.Messages.Experiences;

namespace PsychedelicExperience.Psychedelics.ExperienceStatisticsView
{
    public class ExperienceStatistics
    {
        public Guid Id { get; set; }

        public Counter Total { get; set; } = new Counter();
        public CounterList Privacy { get; set; } = new CounterList();

        public Counter Doses { get; set; } = new Counter();

        public CounterList Substance { get; set; } = new CounterList();
        public CounterList Tags { get; set; } = new CounterList();

        public Counter PrivacyCounter(PrivacyLevel level)
        {
            return Privacy.Get(level.ToString());
        }
    }

    public class CounterList : Dictionary<string, Counter>
    {
        internal void Add(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var counter = Get(name);
            counter.Add();
        }

        internal new void Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var counter = Get(name);
            counter.Remove();
        }

        public Counter Get(string name)
        {
            Counter result;
            if (!TryGetValue(name, out result))
            {
                result = new Counter();
                Add(name, result);
            }
            return result;
        }

        public IDictionary<string, int> Top5()
        {
            return this.OrderByDescending(pair => pair.Value.Value)
                .Take(5)
                .ToDictionary(pair => pair.Key, pair => pair.Value.Value);
        }
    }

    public class Counter
    {
        public int Value { get; set; }

        public void Add() => Value++;
        public void Remove() => Value--;
    }
}