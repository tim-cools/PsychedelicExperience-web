using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserProfileView;
using PsychedelicExperience.Psychedelics.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.Statistics;
using PsychedelicExperience.Psychedelics.Messages.Statistics.Commands;

namespace PsychedelicExperience.Psychedelics.UserInteractions.Handlers
{
    public class GetStatisticsHandler : CommandHandler<GetStatistics, Statistics>
    {
        private const string UserRegisteredEvent = "user_registered";

        private readonly IUserDataProtector _userDataProtection;

        public GetStatisticsHandler(IDocumentSession session, IUserDataProtector userDataProtection) : base(session)
        {
            _userDataProtection = userDataProtection;
        }

        protected override async Task<Statistics> Execute(GetStatistics query)
        {
            if (query.Format == Format.Csv)
            {
                return await GetCsv(query.Category);
            }

            return await GetJson();
        }

        private async Task<Statistics> GetJson()
        {
            var users = await Session.Query<UserProfile>()
                .CountAsync();

            var usersPerMonth = await EventTypePerMonth(UserRegisteredEvent);

            var organisations = await Session.Query<OrganisationView.Organisation>()
                .Where((organisation) => !organisation.Removed)
                .CountAsync();

            var organisationsRemoved = await Session.Query<OrganisationView.Organisation>()
                .Where((organisation) => organisation.Removed)
                .CountAsync();

            var organisationsPerMonth = await EventTypePerMonth("organisation_added");


            var organisationsContacted = await Session.Events.QueryRawEventDataOnly<OrganisationContacted>()
                .CountAsync();

            var organisationsContactedPerMonth = await EventTypePerMonth("organisation_contacted");

            var reviews = await Session.Query<OrganisationView.Review>()
                .Where(review => !review.Removed)
                .CountAsync();

            var reviewsRemoved = await Session.Query<OrganisationView.Review>()
                .Where(review => review.Removed)
                .CountAsync();

            var reviewsPerMonth = await EventTypePerMonth("organisation_review_added");

            var events = await EventCounters();

            return new JsonStatistics(true)
            {
                Users = new UserStatistics
                {
                    Active = users,
                    PerMonth = usersPerMonth
                },
                Organisations = new OrganisationStatistics
                {
                    Active = organisations,
                    Removed = organisationsRemoved,
                    PerMonth = organisationsPerMonth,
                    Contacted = organisationsContacted,
                    ContactedPerMonth = organisationsContactedPerMonth
                },
                Reviews = new OrganisationReviewStatistics
                {
                    Active = reviews,
                    Removed = reviewsRemoved,
                    PerMonth = reviewsPerMonth
                },
                Events = events.ToArray()
            };
        }

        private async Task<CsvStatistics> GetCsv(Category category)
        {
            if (category != Category.Users) throw new InvalidOperationException($"Invalid category: {category}");

            var data = await EventTypePerDay(UserRegisteredEvent);

            var writer = new CsvWriter();
            foreach (var counter in data)
            {
                writer.Write("date", counter.Type);
                writer.Write(UserRegisteredEvent, counter.Counter.ToString());
                writer.EndLine();
            }

            return new CsvStatistics(true)
            {
                Bytes = Encoding.Default.GetBytes(writer.ToString()),
                FileName = $"PEx-{category}-{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv",
            };
        }

        private async Task<IReadOnlyList<EventCounter>> EventCounters()
        {
            const string sql = @"select jsonb_build_object('Type', type,  'Counter', counter)	
from (
	SELECT distinct type, count(*) as counter FROM psychedelics_events.mt_events group by type order by count(*)
) as events_counters;";

            return await Session.QueryAsync<EventCounter>(sql);
        }

        private async Task<MonthCounter[]> EventTypePerMonth(string empty)
        {
            var sql = $@"SELECT jsonb_build_object(
        'month', to_char(DATE_TRUNC('month', timestamp), 'YY-MM'),
        'counter', COUNT(*))
                FROM psychedelics_events.mt_events
            WHERE type='{empty}'
            GROUP BY DATE_TRUNC('month', timestamp)
            ORDER BY DATE_TRUNC('month', timestamp) DESC;";

            return (await Session.QueryAsync<MonthCounter>(sql)).ToArray();
        }

        private async Task<EventCounter[]> EventTypePerDay(string empty)
        {
            var sql = $@"SELECT jsonb_build_object(
        'type', to_char(DATE_TRUNC('day', timestamp), 'YY-MM-DD'),
        'counter', COUNT(*))
                FROM psychedelics_events.mt_events
            WHERE type='{empty}'
            GROUP BY DATE_TRUNC('day', timestamp)
            ORDER BY DATE_TRUNC('day', timestamp) DESC;";

            return (await Session.QueryAsync<EventCounter>(sql)).ToArray();
        }
    }
}
