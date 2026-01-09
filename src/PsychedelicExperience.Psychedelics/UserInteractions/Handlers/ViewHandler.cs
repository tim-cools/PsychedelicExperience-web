using System.Threading.Tasks;
using Marten;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands;

namespace PsychedelicExperience.Psychedelics.UserInteractions.Handlers
{
    public class ViewHandler : CommandHandler<View, Result>
    {
        public ViewHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(View command)
        {
            if (command.UserId == null)
            {
                return await CountAnonymous();
            }

            var userId = command.UserId?.Value;

            //another hack, used the aggregate as identity map, should be stored in a separeted identity map
            var interaction = await Session.Query<UserInteraction>()
                .FirstOrDefaultAsync(where => where.TopicId == command.Id.Value
                                           && where.UserId == userId);

            var id = interaction?.Id ?? CombGuidIdGeneration.NewGuid();
            var aggregate = await Session.LoadAggregate<UserInteraction>(id);

            aggregate.View(
                new UserInteractionId(id), 
                command.Id, 
                command.UserId);

            Session.StoreChanges(aggregate);

            return Result.Success;
        }

        private Task<Result> CountAnonymous()
        {
            //todo implement CountAnonymous
            return Task.FromResult(Result.Success);
        }
    }
}