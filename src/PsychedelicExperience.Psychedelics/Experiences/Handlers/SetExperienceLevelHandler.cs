using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class SetExperienceLevelHandler : CommandHandler<SetExperienceLevel, Result>
    {
        public SetExperienceLevelHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(SetExperienceLevel command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);

            var level = MapLevel(command.Level);
            aggregate.SetExperienceLevel(user, level);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }

        private ExperienceLevel MapLevel(Messages.Experiences.ExperienceLevel level)
        {
            return (ExperienceLevel) level;
        }
    }
}