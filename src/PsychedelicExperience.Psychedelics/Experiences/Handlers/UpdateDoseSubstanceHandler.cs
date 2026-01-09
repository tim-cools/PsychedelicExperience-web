using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class UpdateDoseSubstanceHandler : CommandHandler<UpdateDoseSubstance, Result>
    {
        public UpdateDoseSubstanceHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(UpdateDoseSubstance command)
        {
            var aggregate = await Session.LoadAggregate<Dose>(command.DoseId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.UpdateSubstance(user, command.Substance);

            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}