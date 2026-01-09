using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class RemoveExperienceHandler : CommandHandler<RemoveExperience, Result>
    {
        public RemoveExperienceHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(RemoveExperience command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);
            
            aggregate.Remove(user);
            Session.StoreChanges(aggregate);

            Session.Delete(aggregate);

            return Result.Success;
        }
    }
}