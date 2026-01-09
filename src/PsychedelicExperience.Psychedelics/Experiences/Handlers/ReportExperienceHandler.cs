using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class ReportExperienceHandler : CommandHandler<ReportExperience, Result>
    {
        public ReportExperienceHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(ReportExperience command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.Report(user, command.Reason);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}
