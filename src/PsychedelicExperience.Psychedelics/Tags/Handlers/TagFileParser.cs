using System;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;

namespace PsychedelicExperience.Psychedelics.Tags.Handlers
{
    internal class TagFileParser
    {
        private const string CategoryPrefix = "Category:";
        private const string SubCategoryPrefix = "SubCategory:";

        private readonly Action<Tag> _addTag;
        private string _category;
        private string _subCategory;

        public TagFileParser(Action<Tag> addTag)
        {
            _addTag = addTag;
        }

        public void Parse(string line)
        {
            if (line.StartsWith(CategoryPrefix))
            {
                _category = line.Substring(CategoryPrefix.Length);
                _subCategory = null;
            }
            else if (line.StartsWith(SubCategoryPrefix))
            {
                _subCategory = line.Substring(SubCategoryPrefix.Length);
            }
            else
            {
                _addTag(new Tag(_category, _subCategory, line));
            }
        }
    }
}