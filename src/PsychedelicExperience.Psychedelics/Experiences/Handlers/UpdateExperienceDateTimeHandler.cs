using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class UpdateExperienceDateTimeHandler : CommandHandler<UpdateExperienceDateTime, Result>
    {
        public UpdateExperienceDateTimeHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(UpdateExperienceDateTime command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.UpdateDateTime(user, command.DateTime);

            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}
