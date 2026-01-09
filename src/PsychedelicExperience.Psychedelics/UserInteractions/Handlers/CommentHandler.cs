using System.Threading.Tasks;
using Marten;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Commands;

namespace PsychedelicExperience.Psychedelics.UserInteractions.Handlers
{
    public class CommentHandler : CommandHandler<Comment, Result>
    {
        private readonly IUserDataProtector _userDataProtection;

        public CommentHandler(IDocumentSession session, IUserDataProtector userDataProtection) : base(session)
        {
            _userDataProtection = userDataProtection;
        }

        protected override async Task<Result> Execute(Comment command)
        {
            if (command.UserId == null) throw new BusinessException("User is obligated");

            //another hack, used the aggregate as identity map, should be stored in a separeted identity map
            var interaction = await Session.Query<UserInteraction>()
                .FirstOrDefaultAsync(where => where.TopicId == command.Id.Value
                                           && where.UserId == command.UserId.Value);

            var id = interaction?.Id ?? CombGuidIdGeneration.NewGuid();
            var aggregate = await Session.LoadAggregate<UserInteraction>(id);
            var encrypted = _userDataProtection.Encrypt(command.UserId, command.Text);
            
            aggregate.Comment(
                new UserInteractionId(id), 
                command.Id,
                command.UserId, 
                encrypted);

            Session.StoreChanges(aggregate);

            return Result.Success;
        }
    }
}