using System;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Queries
{
    public class EventsSitemapResult
    {
        public EventsSitemapEntry[] Events { get; set; }
        public string[] Countries { get; set; }
        public EventType[] EventTypes { get; set; }
        public CountryType[] CountriesAndTypes { get; set; }
    }

    public class CountryType
    {
        public string County { get; set; }
        public EventType EventType { get; set; }

        public CountryType(EventsSitemapEntry entry)
        {
            County = entry.Country;
            EventType = entry.EventType;
        }

        protected bool Equals(CountryType other)
        {
            return County == other.County && EventType == other.EventType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CountryType) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(County, (int) EventType);
        }
    }
}