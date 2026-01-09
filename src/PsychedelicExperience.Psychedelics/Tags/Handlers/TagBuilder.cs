using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;

namespace PsychedelicExperience.Psychedelics.Tags.Handlers
{

    internal class TagBuilder
    {
        private readonly IList<Tag> _tags = new List<Tag>();

        public Tag[] Tags => _tags.ToArray();

        public CategoryBuilder WithCategory(string category)
        {
            return new CategoryBuilder(category, this);
        }

        public void AddTag(Tag tag) => _tags.Add(tag);

        public static implicit operator Tag[] (TagBuilder builder)
        {
            return builder.Tags;
        }
    }
}
