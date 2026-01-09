using System.Collections.Generic;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Commands;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.Messages.Infrastructure;

namespace PsychedelicExperience.Psychedelics.Infrastructure
{
    public class InitializeDatabaseHandler : CommandHandler<InitializeDatabaseCommand>
    {
        private readonly IMediator _mediator;

        public InitializeDatabaseHandler(IDocumentSession session, IMediator mediator)
            : base(session)
        {
            _mediator = mediator;
        }

        protected override async Task<Result> Execute(InitializeDatabaseCommand command)
        {
            await InitializeExperiences();

            return Result.Success;
        }

        private async Task InitializeExperiences()
        {
            var userId = await AddUser();
            var tasks = new List<Task>();
            for (var index = 0; index < 200; index++)
            {
                tasks.Add(AddExperience(userId, index));
            }
            await Task.WhenAll(tasks);
        }

        private async Task<UserId> AddUser()
        {
            var email = new EMail("0123456789$@you.com");
            var command = new RegisterUserCommand(
                new Name("Terrence"),
                new Name("Terrence Mck"),
                email,
                new Password("Aa-0123456789$"),
                new Password("Aa-0123456789$"));

            await _mediator.Send(command);

            var user = await _mediator.Send(new UserByEMailQuery(null, email));

            return new UserId(user.Id);
        }

        private Task AddExperience(UserId userId, int index)
        {
            var experienceId = ExperienceId.New();
            var doseId = DoseId.New();

            var substance = Substances.Get(index);

            var data = new ExperienceData();
            data.AddValue("title", "Experience " + index);
            data.AddValue("description", "description " + index);

            return _mediator.Send(new AddExperience(userId, experienceId, data))
                .ContinueWith(state => _mediator.Send(new SetExperiencePrivacy(experienceId, userId, PrivacyLevel.Public)))
                .ContinueWith(state => _mediator.Send(new AddDose(experienceId, userId, doseId)))
                .ContinueWith(state => _mediator.Send(new UpdateDoseSubstance(userId, doseId, substance)));
        }
    }
}