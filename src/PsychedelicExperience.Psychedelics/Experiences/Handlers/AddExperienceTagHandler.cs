using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Aggregates;
using AddExperienceTag = PsychedelicExperience.Psychedelics.Messages.Experiences.Commands.AddExperienceTag;
using PsychedelicExperience.Membership;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class AddExperienceTagHandler : CommandHandler<AddExperienceTag, Result>
    {
        public AddExperienceTagHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(AddExperienceTag command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);

            aggregate.AddTag(user, command.TagName);

            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}