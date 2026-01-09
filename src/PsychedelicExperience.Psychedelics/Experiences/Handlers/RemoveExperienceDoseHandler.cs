using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class RemoveExperienceDoseHandler : CommandHandler<RemoveDose, Result>
    {
        public RemoveExperienceDoseHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(RemoveDose command)
        {
            var aggregate = await Session.LoadAggregate<Dose>(command.DoseId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.Remove(user);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}