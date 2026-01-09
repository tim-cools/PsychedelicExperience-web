using System;
using System.Linq;
using Baseline;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;

namespace PsychedelicExperience.Psychedelics.Tags.Handlers
{
    internal class CategoryBuilder
    {
        private readonly string _category;
        private readonly string _subCategory;
        private readonly TagBuilder _tagBuilder;

        public CategoryBuilder(string category, TagBuilder tagBuilder) : this(category, null, tagBuilder)
        {
        }

        public CategoryBuilder(string category, string subCategory, TagBuilder tagBuilder)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(category));
            }

            _category = category;
            _subCategory = subCategory;
            _tagBuilder = tagBuilder;
        }

        public CategoryBuilder WithCategory(string category)
        {
            return new CategoryBuilder(category, _tagBuilder);
        }

        public CategoryBuilder WithSubCategory(string subCategory)
        {
            return new CategoryBuilder(_category, subCategory, _tagBuilder);
        }

        public CategoryBuilder WithTags(params string[] tags)
        {
            tags.Select(tag => new Tag(_category, _subCategory, tag))
                .Each(_tagBuilder.AddTag);
            return this;
        }

        public TagBuilder LoadFromTextFile(string name)
        {
            var lines = typeof(TagBuilder).ReadResourceLines(name);
            var parser = new TagFileParser(_tagBuilder.AddTag);
            foreach (var line in lines)
            {
                parser.Parse(line.Trim());
            }
            return _tagBuilder;
        }

        public static implicit operator Tag[] (CategoryBuilder builder)
        {
            return builder._tagBuilder.Tags;
        }
    }
}