using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class AddDoseHandler : CommandHandler<AddDose, Result>
    {
        public AddDoseHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(AddDose command)
        {
            var experience = await Session.LoadAggregate<Experience>(command.ExperienceId);

            var aggregate = await Session.LoadAggregate<Dose>(command.DoseId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.Add(command.DoseId, experience, user);

            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}