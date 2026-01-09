using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    public class GetEventsSitemapHandler : QueryHandler<GetEventsSitemap, EventsSitemapResult>
    {
        public GetEventsSitemapHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<EventsSitemapResult> Execute(GetEventsSitemap getEventsQuery)
        {
            var events = await Session.Query<Event>()
                .Where(@event => !@event.Removed && @event.Privacy == EventPrivacy.Public )
                .Select(@event => new {@event.Id, @event.Name, @event.Tags, @event.Address.Country, @event.EventType})
                .ToListAsync();
            
            var entries = events.Select(@event => new EventsSitemapEntry()
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    Country = @event.Country,
                    EventType = @event.EventType.CastByName<Messages.Events.EventType>(),
                    Tags = @event.Tags.Select(tag => tag).ToArray()
                })
                .ToArray();

            var tags = NormalizeExisting(entries, @event => @event.Tags);
            var countries = NormalizeExisting(entries, @event => @event.Country);
            var eventTypes = NormalizeExisting(entries, @event => @event.EventType);
            var countryTypes = NormalizeExisting(entries.Where(entry => !string.IsNullOrEmpty(entry.Country)).ToArray(), @event => new CountryType(@event));

            return new EventsSitemapResult
            {
                Events = entries,
                Countries = countries,
                EventTypes = eventTypes,
                CountriesAndTypes = countryTypes,
            };
        }

        private static T[] NormalizeExisting<T>(EventsSitemapEntry[] events, Func<EventsSitemapEntry, T> propertySelector)
        {
            return events
                .Select(propertySelector)
                .Distinct()
                .Where(value => value != null)
                .ToArray();
        }
        private static string[] NormalizeExisting(EventsSitemapEntry[] events, Func<EventsSitemapEntry, IEnumerable<string>> collectionSelector)
        {
            return events
                .SelectMany(collectionSelector)
                .Distinct()
                .Where(value => value != null)
                .Select(value => value.NormalizeForUrl())
                .ToArray();
        }
    }
}
