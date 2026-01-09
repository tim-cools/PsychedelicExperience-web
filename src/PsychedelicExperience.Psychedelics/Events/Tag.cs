using PsychedelicExperience.Membership.Messages;

namespace PsychedelicExperience.Psychedelics.Events
{
    public class Tag
    {
        public Name Name { get; }

        public Tag(Name name)
        {
            Name = name;
        }
    }
}