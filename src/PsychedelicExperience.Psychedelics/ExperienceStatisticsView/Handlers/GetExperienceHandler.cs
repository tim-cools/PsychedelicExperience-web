using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;

namespace PsychedelicExperience.Psychedelics.ExperienceStatisticsView.Handlers
{
    public class GetExperienceStatisticsHandler : QueryHandler<GetExperienceStatistics, Messages.Experiences.Queries.ExperienceStatistics>
    {
        public GetExperienceStatisticsHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<Messages.Experiences.Queries.ExperienceStatistics> Execute(GetExperienceStatistics query)
        {
            var experience = await Session.LoadAsync<ExperienceStatistics>(ExperienceStatisticsProjection.StatisticsId);

            return experience?.Map();
        }
    }
}
