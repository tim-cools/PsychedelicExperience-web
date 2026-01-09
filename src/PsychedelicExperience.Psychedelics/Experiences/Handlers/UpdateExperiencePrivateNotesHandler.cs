using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;

namespace PsychedelicExperience.Psychedelics.Experiences.Handlers
{
    public class UpdateExperiencePrivateNotesHandler : CommandHandler<UpdateExperiencePrivateNotes, Result>
    {
        private IUserDataProtector _userDataProtector;

        public UpdateExperiencePrivateNotesHandler(IDocumentSession session, IUserDataProtector userDataProtector) : base(session)
        {
            _userDataProtector = userDataProtector;
        }

        protected override async Task<Result> Execute(UpdateExperiencePrivateNotes command)
        {
            var aggregate = await Session.LoadAggregate<Experience>(command.ExperienceId);
            var user = await Session.LoadUserAsync(command.UserId);

            var description = _userDataProtector.Encrypt(command.UserId, command.Description.Value);

            aggregate.SetPrivateNotes(user, description);
            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}