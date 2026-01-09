using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Marten;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Organisations;
using ValidationExtensions = PsychedelicExperience.Psychedelics.OrganisationUpdates.ValidationExtensions;

namespace PsychedelicExperience.Psychedelics.Events.Commands
{
    public abstract class EventHandler<TEvent> : AggregateCommandHandler<TEvent, Event, EventId, EventHandler<TEvent>.Context, Result> where TEvent : IRequest<Result>
    {
        private readonly Func<TEvent, OrganisationId> _organisationIdSelector;

        public class Context : Context<TEvent, Event>
        {
            public Organisation Organisation { get; set; }
        }

        protected EventHandler(IDocumentSession session,
            IValidator<TEvent> validatorCommand,
            Func<TEvent, EventId> aggregateIdSelector,
            Func<TEvent, UserId> userIdSelector,
            Action<Context> authenticate) : this(session, validatorCommand, aggregateIdSelector, userIdSelector, null, authenticate)
        {
        }

        protected EventHandler(IDocumentSession session,
            IValidator<TEvent> validatorCommand,
            Func<TEvent, EventId> aggregateIdSelector,
            Func<TEvent, UserId> userIdSelector,
            Func<TEvent, OrganisationId> organisationIdSelector,
            Action<Context> authenticate) : base(session, validatorCommand, aggregateIdSelector, userIdSelector, authenticate)
        {
            _organisationIdSelector = organisationIdSelector;
        }

        protected override Result ResultSuccess(Context context) 
            => Result.Success;

        protected override Result ResultFailed(Context context, BusinessException businessException) 
            => Result.Failed(businessException.Errors.ToArray());

        protected override async Task LoadData(Context context)
        {
            await base.LoadData(context);

            var organisationId = _organisationIdSelector != null  
                ? (Guid)_organisationIdSelector(context.Command)
                : (Guid)context.Aggregate.OrganisationId;

            context.Organisation = await Session.LoadAsync<Organisation>(organisationId);
        }
    }

    public class AddEventValidator : AbstractValidator<AddEvent>
    {
        public AddEventValidator()
        {
            RuleFor(command => command.UserId).UserId();
            ValidationExtensions.OrganisationId(RuleFor(command => command.OrganisationId));
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.Privacy).Privacy();
            RuleFor(command => command.EventType).EventType();
            RuleFor(command => command.StartDateTime).DateTimeNotNull();
            RuleFor(command => command.Location).Location();
            RuleFor(command => command.Name).Name();
            RuleFor(command => command.Description).Description();
        }
    }

    public class AddEventHandler : EventHandler<AddEvent>
    {
        public AddEventHandler(IDocumentSession session, IValidator<AddEvent> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                command => command.OrganisationId,
                context => context.Aggregate.EnsureCanAdd(context.User, context.Organisation))
        {
        }
    }

    public class RemoveEventValidator : AbstractValidator<RemoveEvent>
    {
        public RemoveEventValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
        }
    }

    public class RemoveEventHandler : EventHandler<RemoveEvent>
    {
        public RemoveEventHandler(IDocumentSession session, IValidator<RemoveEvent> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanRemove(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventStartDateTimeValidator : AbstractValidator<ChangeEventStartDateTime>
    {
        public ChangeEventStartDateTimeValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.DateTime).DateTimeNotNull();
        }
    }

    public class ChangeEventStartDateTimeHandler : EventHandler<ChangeEventStartDateTime>
    {
        public ChangeEventStartDateTimeHandler(IDocumentSession session, IValidator<ChangeEventStartDateTime> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventEndDateTimeValidator : AbstractValidator<ChangeEventEndDateTime>
    {
        public ChangeEventEndDateTimeValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
        }
    }

    public class ChangeEventEndDateTimeHandler : EventHandler<ChangeEventEndDateTime>
    {
        public ChangeEventEndDateTimeHandler(IDocumentSession session, IValidator<ChangeEventEndDateTime> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventLocationValidator : AbstractValidator<ChangeEventLocation>
    {
        public ChangeEventLocationValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.Location).Location();
        }
    }

    public class ChangeEventLocationHandler : EventHandler<ChangeEventLocation>
    {
        public ChangeEventLocationHandler(IDocumentSession session, IValidator<ChangeEventLocation> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventNameValidator : AbstractValidator<ChangeEventName>
    {
        public ChangeEventNameValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.Name).Name();
        }
    }

    public class ChangeEventNameHandler : EventHandler<ChangeEventName>
    {
        public ChangeEventNameHandler(IDocumentSession session, IValidator<ChangeEventName> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventDescriptionValidator : AbstractValidator<ChangeEventDescription>
    {
        public ChangeEventDescriptionValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.Description).Description();
        }
    }

    public class ChangeEventDescriptionHandler : EventHandler<ChangeEventDescription>
    {
        public ChangeEventDescriptionHandler(IDocumentSession session, IValidator<ChangeEventDescription> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventImageValidator : AbstractValidator<ChangeEventImage>
    {
        public ChangeEventImageValidator()
        {
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Image).Image();
        }
    }

    public class ChangeEventImageHandler : EventHandler<ChangeEventImage>
    {
        public ChangeEventImageHandler(IDocumentSession session, IValidator<ChangeEventImage> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ClearEventImageValidator : AbstractValidator<ClearEventImage>
    {
        public ClearEventImageValidator()
        {
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class ClearEventImageHandler : EventHandler<ClearEventImage>
    {
        public ClearEventImageHandler(IDocumentSession session, IValidator<ClearEventImage> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventPrivacyValidator : AbstractValidator<ChangeEventPrivacy>
    {
        public ChangeEventPrivacyValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.Privacy).Privacy();
        }
    }

    public class ChangeEventPrivacyHandler : EventHandler<ChangeEventPrivacy>
    {
        public ChangeEventPrivacyHandler(IDocumentSession session, IValidator<ChangeEventPrivacy> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventTypeValidator : AbstractValidator<ChangeEventType>
    {
        public ChangeEventTypeValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.EventType).EventType();
        }
    }

    public class ChangeEventTypeHandler : EventHandler<ChangeEventType>
    {
        public ChangeEventTypeHandler(IDocumentSession session, IValidator<ChangeEventType> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class AddEventTagValidator : AbstractValidator<AddEventTag>
    {
        public AddEventTagValidator()
        {
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.TagName).TagName();
        }
    }

    public class AddEventTagHandler : EventHandler<AddEventTag>
    {
        public AddEventTagHandler(IDocumentSession session, IValidator<AddEventTag> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class RemoveEventTagValidator : AbstractValidator<RemoveEventTag>
    {
        public RemoveEventTagValidator()
        {
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.TagName).TagName();
        }
    }

    public class RemoveEventTagHandler : EventHandler<RemoveEventTag>
    {
        public RemoveEventTagHandler(IDocumentSession session, IValidator<RemoveEventTag> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }

    public class ChangeEventMemberStatusValidator : AbstractValidator<ChangeEventMemberStatus>
    {
        public ChangeEventMemberStatusValidator()
        {
            RuleFor(command => command.MemberId).MemberId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Status).Status();
        }
    }

    public class ChangeEventMemberStatusHandler : EventMemberHandler<ChangeEventMemberStatus>
    {
        public ChangeEventMemberStatusHandler(IDocumentSession session, IValidator<ChangeEventMemberStatus> commandValidator) :
            base(session, commandValidator,
                command => command.MemberId,
                command => command.UserId,
                command => command.EventId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Event, context.Organisation))
        {
        }
    }

    public class InviteEventMemberValidator : AbstractValidator<InviteEventMember>
    {
        public InviteEventMemberValidator()
        {
            RuleFor(command => command.MemberId).MemberId();
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.MemberId).MemberId();
        }
    }

    public abstract class EventMemberHandler<TEvent> : AggregateCommandHandler<TEvent, EventMember, UserId, EventMemberHandler<TEvent>.Context, Result> where TEvent : IRequest<Result>
    {
        private readonly Func<TEvent, EventId> _eventIdSelector;

        public class Context : Context<TEvent, EventMember>
        {
            public Event Event { get; set; }
            public Organisation Organisation { get; set; }
        }

        //protected EventMemberHandler(IDocumentSession session,
        //    IValidator<TEvent> validatorCommand,
        //    Func<TEvent, UserId> aggregateIdSelector,
        //    Func<TEvent, UserId> userIdSelector,
        //    Action<Context> authenticate) : this(session, validatorCommand, aggregateIdSelector, userIdSelector, null, authenticate)
        //{
        //}

        protected EventMemberHandler(IDocumentSession session,
            IValidator<TEvent> validatorCommand,
            Func<TEvent, UserId> aggregateIdSelector,
            Func<TEvent, UserId> userIdSelector,
            Func<TEvent, EventId> eventIdSelector,
            Action<Context> authenticate) : base(session, validatorCommand, aggregateIdSelector, userIdSelector, authenticate)
        {
            _eventIdSelector = eventIdSelector;
        }

        protected override Result ResultSuccess(Context context) 
            => Result.Success;

        protected override Result ResultFailed(Context context, BusinessException businessException) 
            => Result.Failed(businessException.Errors.ToArray());
        
        protected override async Task LoadData(Context context)
        {
            await base.LoadData(context);

            var eventId = EventId(context);

            context.Event = await Session.LoadAsync<Event>(eventId);
            context.Organisation = await Session.LoadAsync<Organisation>((Guid) context.Event.OrganisationId);
        }

        private Guid EventId(Context context)
        {
            return _eventIdSelector != null
                ? (Guid) _eventIdSelector(context.Command)
                : context.Aggregate.EventId;
        }

        protected override async Task LoadAggregate(Context context, Func<TEvent, UserId> aggregateIdSelector)
        {
            var memberId = await GetEventMemberId(context, aggregateIdSelector);

            context.Aggregate = await Session.LoadAggregate<EventMember>(memberId);
        }

        private async Task<Guid> GetEventMemberId(Context context, Func<TEvent, UserId> aggregateIdSelector)
        {
            var memberId = aggregateIdSelector(context.Command);
            if (memberId == null)
            {
                throw new InvalidOperationException("aggregateId should not be null");
            }

            var memberIdValue = memberId.Value;
            var eventIdValue = EventId(context);

            //another hack, used the aggregate as identity map, should be stored in a separeted identity map
            var member = await Session.Query<EventMember>()
                .FirstOrDefaultAsync(where => where.EventId == eventIdValue
                                            && where.MemberId == memberIdValue);

            return member?.Id ?? CombGuidIdGeneration.NewGuid();
        }
    }

    public class InviteEventMemberHandler : EventMemberHandler<InviteEventMember>
    {
        public InviteEventMemberHandler(IDocumentSession session, IValidator<InviteEventMember> commandValidator) :
            base(session, commandValidator,
                command => command.MemberId,
                command => command.UserId,
                command => command.EventId,
                context => context.Aggregate.EnsureCanJoin(context.User, context.Event, context.Organisation))
        {
        }
    }

    public class JoinEventMemberValidator : AbstractValidator<JoinEventMember>
    {
        public JoinEventMemberValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Status).Status();
        }
    }

    public class JoinEventMemberHandler : EventMemberHandler<JoinEventMember>
    {
        public JoinEventMemberHandler(IDocumentSession session, IValidator<JoinEventMember> commandValidator) :
            base(session, commandValidator,
                command => command.UserId,
                command => command.UserId,
                command => command.EventId,
                context => context.Aggregate.EnsureCanJoin(context.User, context.Event, context.Organisation))
        {
        }
    }

    public class RemoveEventMemberValidator : AbstractValidator<RemoveEventMember>
    {
        public RemoveEventMemberValidator()
        {
            RuleFor(command => command.MemberId).MemberId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class RemoveEventMemberHandler : EventMemberHandler<RemoveEventMember>
    {
        public RemoveEventMemberHandler(IDocumentSession session, IValidator<RemoveEventMember> commandValidator) :
            base(session, commandValidator,
                command => command.MemberId,
                command => command.UserId,
                command => command.EventId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Event, context.Organisation))
        {
        }
    }

    public class ReportEventValidator : AbstractValidator<ReportEvent>
    {
        public ReportEventValidator()
        {
            RuleFor(command => command.EventId).EventId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Reason).Reason();
        }
    }

    public class ReportEventHandler : EventHandler<ReportEvent>
    {
        public ReportEventHandler(IDocumentSession session, IValidator<ReportEvent> commandValidator) :
            base(session, commandValidator,
                command => command.EventId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanEdit(context.User, context.Organisation))
        {
        }
    }
}