using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class UpdateDoseUnitHandler : CommandHandler<UpdateDoseUnit, Result>
    {
        public UpdateDoseUnitHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(UpdateDoseUnit command)
        {
            var aggregate = await Session.LoadAggregate<Dose>(command.DoseId);
            var user = await Session.LoadUserAsync(command.UserId);

            //todo check authorized
            aggregate.UpdateUnit(user, command.Unit);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}