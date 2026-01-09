using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands;
using PsychedelicExperience.Psychedelics.OrganisationView;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdates
{
    public class AddOrganisationUpdateValidator : AbstractValidator<AddOrganisationUpdate>
    {
        public AddOrganisationUpdateValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UpdateId).OrganisationUpdateId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Subject).Subject();
            RuleFor(command => command.Content).Content();
        }
    }

    public abstract class OrganisationUpdatesHandler<TEvent> : AggregateCommandHandler<TEvent, OrganisationUpdate, OrganisationUpdateId, OrganisationUpdatesHandler<TEvent>.Context, Result> where TEvent : IRequest<Result>
    {
        private readonly Func<TEvent, OrganisationId> _organisationIdSelector;

        public class Context : Context<TEvent, OrganisationUpdate>
        {
            public Organisation Organisation { get; set; }
        }

        protected OrganisationUpdatesHandler(IDocumentSession session, 
            IValidator<TEvent> validatorCommand, 
            Func<TEvent, OrganisationUpdateId> aggregateIdSelector, 
            Func<TEvent, UserId> userIdSelector, 
            Func<TEvent, OrganisationId> organisationIdSelector,
            Action<Context> authenticate) : base(session, validatorCommand, aggregateIdSelector, userIdSelector, authenticate)
        {
            _organisationIdSelector = organisationIdSelector;
        }

        protected override Result ResultSuccess(Context context) => Result.Success;

        protected override Result ResultFailed(Context context, BusinessException businessException)
        {
            return Result.Failed(businessException.Errors.ToArray());
        }

        protected override async Task LoadData(Context context)
        {
            await base.LoadData(context);

            var organisationId = _organisationIdSelector(context.Command);

            context.Organisation = await Session.LoadAsync<Organisation>((Guid)organisationId);
        }
    }

    public class AddOrganisationUpdateHandler : OrganisationUpdatesHandler<AddOrganisationUpdate>
    {
        public AddOrganisationUpdateHandler(IDocumentSession session, IValidator<AddOrganisationUpdate> commandValidator) :
            base(session, commandValidator,
                command => command.UpdateId,
                command => command.UserId,
                command => command.OrganisationId,
                context => context.Aggregate.EnsureCanAdd(context.User, context.Organisation))
        {
        }
    }

    public class RemoveOrganisationUpdateValidator : AbstractValidator<RemoveOrganisationUpdate>
    {
        public RemoveOrganisationUpdateValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UpdateId).OrganisationUpdateId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class RemoveOrganisationUpdateHandler : OrganisationUpdatesHandler<RemoveOrganisationUpdate>
    {
        public RemoveOrganisationUpdateHandler(IDocumentSession session, IValidator<RemoveOrganisationUpdate> commandValidator) :
            base(session, commandValidator,
                command => command.UpdateId,
                command => command.UserId,
                command => command.OrganisationId,
                context => context.Aggregate.EnsureCanRemove(context.User, context.Organisation))
        {
        }

        protected override async Task StoreAggregate(Context context)
        {
            await base.StoreAggregate(context);

            Session.Delete(context.Aggregate);
        }
    }

    public class SetOrganisationUpdateSubjectValidator : AbstractValidator<SetOrganisationUpdateSubject>
    {
        public SetOrganisationUpdateSubjectValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UpdateId).OrganisationUpdateId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Subject).Subject();
        }
    }

    public class SetOrganisationUpdateSubjectHandler : OrganisationUpdatesHandler<SetOrganisationUpdateSubject>
    {
        public SetOrganisationUpdateSubjectHandler(IDocumentSession session, IValidator<SetOrganisationUpdateSubject> commandValidator) :
            base(session, commandValidator,
                command => command.UpdateId,
                command => command.UserId,
                command => command.OrganisationId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class SetOrganisationUpdateContentValidator : AbstractValidator<SetOrganisationUpdateContent>
    {
        public SetOrganisationUpdateContentValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UpdateId).OrganisationUpdateId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Content).Content();
        }
    }

    public class SetOrganisationUpdateContentHandler : OrganisationUpdatesHandler<SetOrganisationUpdateContent>
    {
        public SetOrganisationUpdateContentHandler(IDocumentSession session, IValidator<SetOrganisationUpdateContent> commandValidator) :
            base(session, commandValidator,
                command => command.UpdateId,
                command => command.UserId,
                command => command.OrganisationId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class SetOrganisationUpdatePrivacyValidator : AbstractValidator<SetOrganisationUpdatePrivacy>
    {
        public SetOrganisationUpdatePrivacyValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UpdateId).OrganisationUpdateId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Privacy).Privacy();
        }
    }

    public class SetOrganisationUpdatePrivacyHandler : OrganisationUpdatesHandler<SetOrganisationUpdatePrivacy>
    {
        public SetOrganisationUpdatePrivacyHandler(IDocumentSession session, IValidator<SetOrganisationUpdatePrivacy> commandValidator) :
            base(session, commandValidator,
                command => command.UpdateId,
                command => command.UserId,
                command => command.OrganisationId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }
}