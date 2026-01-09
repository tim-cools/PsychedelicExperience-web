using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Statistics
{
    public class Statistics : Result
    {
        public Statistics()
        {
        }

        protected Statistics(bool success) : base(success)
        {
        }
    }

    public class JsonStatistics : Statistics
    {
        public UserStatistics Users { get; set; }

        public OrganisationStatistics Organisations { get; set; }
        public OrganisationReviewStatistics Reviews { get; set; }
        public EventCounter[] Events { get; set; }

        public JsonStatistics()
        {
        }

        public JsonStatistics(bool success) : base(success)
        {
        }
    }

    public class CsvStatistics : Statistics
    {
        public byte[] Bytes { get; set; }
        public string FileName { get; set; }

        public CsvStatistics()
        {
        }

        public CsvStatistics(bool success) : base(success)
        {
        }
    }

    public class EventCounter
    {
        public string Type { get; set; }
        public int Counter { get; set; }
    }

    public class MonthCounter
    {
        public string Counter { get; set; }
        public string Month { get; set; }
    }

    public class UserStatistics
    {
        public int Active { get; set; }
        public MonthCounter[] PerMonth { get; set; }
    }

    public class OrganisationStatistics
    {
        public int Active { get; set; }
        public int Removed { get; set; }
        public MonthCounter[] PerMonth { get; set; }
        public int Contacted { get; set; }
        public MonthCounter[] ContactedPerMonth { get; set; }
    }

    public class OrganisationReviewStatistics
    {
        public int Active { get; set; }
        public int Removed { get; set; }
        public MonthCounter[] PerMonth { get; set; }
    }
}