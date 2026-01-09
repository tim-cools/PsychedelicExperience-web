using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class SetExperiencePrivacyHandler : CommandHandler<SetExperiencePrivacy, Result>
    {
        public SetExperiencePrivacyHandler(IDocumentSession session) : base(session)
        {
        }

        protected override async Task<Result> Execute(SetExperiencePrivacy command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);          
            
            var level = MapLevel(command.Level);
            aggregate.SetPrivacyLevel(user, level);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }

        private PrivacyLevel MapLevel(Messages.Experiences.PrivacyLevel level)
        {
            return (PrivacyLevel) level;
        }
    }
}