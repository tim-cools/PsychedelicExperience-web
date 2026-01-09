using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Psychedelics.Messages.Tags.Events;

namespace PsychedelicExperience.Psychedelics.Tags.Domain
{
    public class Tag : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }

        public void Add(Guid tagId, Guid userId, string name)
        {
            Publish(new TagAdded
            {
                UserId = userId,
                TagId = tagId,
                Name = name
            });
        }

        public void Apply(TagAdded @event)
        {
            Id = @event.TagId;
            UserId = @event.UserId;
            Name = @event.Name;
        }

        public void UpdateName(Guid userId, string name)
        {
            Publish(new TagNameUpdated
            {
                UserId = userId,
                Name = name
            });
        }

        public void Apply(TagNameUpdated @event)
        {
            Name = @event.Name;
        }

        public void UpdateUrl(Guid userId, string url)
        {
            Publish(new TagUrlUpdated
            {
                UserId = userId,
                Url = url
            });
        }

        public void Apply(TagUrlUpdated @event)
        {
            Url = @event.Url;
        }
    }
}