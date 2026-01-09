using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class UpdateDoseMethodHandler : CommandHandler<UpdateDoseMethod, Result>
    {
        public UpdateDoseMethodHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(UpdateDoseMethod command)
        {
            var aggregate = await Session.LoadAggregate<Dose>(command.DoseId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.UpdateMethod(user, command.Method);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}