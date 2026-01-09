using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class Rating
    {
        public ScaleOf5 Value { get; set; }
        public string Description { get; set; }

        public Rating()
        {
        }

        public Rating(ScaleOf5 value, string description)
        {
            Value = value;
            Description = description;
        }
    }
}