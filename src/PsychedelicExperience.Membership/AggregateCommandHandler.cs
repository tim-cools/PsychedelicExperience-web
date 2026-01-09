using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership
{
    public abstract class AggregateCommandHandler<TCommand, TAggregate, TAggregateId> :
            AggregateCommandHandler<TCommand, TAggregate, TAggregateId, Context<TCommand, TAggregate>, Result>
        where TCommand : IRequest<Result>
        where TAggregate : AggregateRoot, new()
        where TAggregateId : Id
    {
        protected AggregateCommandHandler(
            IDocumentSession session,
            IValidator<TCommand> validatorCommand,
            Func<TCommand, TAggregateId> aggregateIdSelector,
            Func<TCommand, UserId> userIdSelector,
            Action<TAggregate, User> authenticate)
            : base(session,
                  validatorCommand,
                  aggregateIdSelector,
                  userIdSelector,
                  context => authenticate(context.Aggregate, context.User))
        {
        }

        protected override Result ResultSuccess(Context<TCommand, TAggregate> context)
        {
            return Result.Success;
        }

        protected override Result ResultFailed(Context<TCommand, TAggregate> context, BusinessException businessException)
        {
            return Result.Failed(businessException.Errors.ToArray());
        }
    }

    public class Context<TCommand, TAggregate>
    {
        public TCommand Command { get; set; }
        public TAggregate Aggregate { get; set; }
        public User User { get; set; }
    }

    public abstract class AggregateCommandHandler<TCommand, TAggregate, TAggregateId, TContext, TResult> : IAsyncRequestHandler<TCommand, TResult>
        where TCommand : IRequest<TResult>
        where TAggregate : AggregateRoot, new()
        where TAggregateId : Id
        where TContext : Context<TCommand, TAggregate>, new()
        where TResult : Result
    {
        private readonly IValidator<TCommand> _validatorCommand;
        private readonly Func<TCommand, TAggregateId> _aggregateIdSelector;
        private readonly Func<TCommand, UserId> _userIdSelector;
        private readonly Action<TContext> _authenticate;

        protected IDocumentSession Session { get; }

        protected AggregateCommandHandler(
            IDocumentSession session,
            IValidator<TCommand> validatorCommand,
            Func<TCommand, TAggregateId> aggregateIdSelector,
            Func<TCommand, UserId> userIdSelector,
            Action<TContext> authenticate)
        {
            Session = session;
            _validatorCommand = validatorCommand;
            _aggregateIdSelector = aggregateIdSelector;
            _userIdSelector = userIdSelector;
            _authenticate = authenticate;
        }

        public async Task<TResult> Handle(TCommand command)
        {
            var context = new TContext { Command = command };

            await LoadAggregates(context);

            _authenticate(context);

            try
            {
                ValidateCommand(context);
                HandleCommand(context);
                await AdditionalCommand(context);
            }
            catch (BusinessException businessException)
            {
                return ResultFailed(context, businessException);
            }

            StoreAggregate(context);

            await Session.SaveChangesAsync();

            return ResultSuccess(context);
        }

        protected abstract TResult ResultSuccess(Context<TCommand, TAggregate> context);

        protected abstract TResult ResultFailed(Context<TCommand, TAggregate> context, BusinessException businessException);

        private void ValidateCommand(TContext context)
        {
            var result = _validatorCommand.Validate(context.Command);
            if (!result.IsValid)
            {
                throw new BusinessException(result.Errors.Select(Map).ToArray());
            }
        }

        private ValidationError Map(ValidationFailure arg)
        {
            return new ValidationError(arg.PropertyName, arg.ErrorCode, arg.ErrorMessage);
        }

        protected virtual async Task LoadAggregates(TContext context)
        {
            var aggregateId = _aggregateIdSelector(context.Command);
            if (aggregateId == null)
            {
                throw new InvalidOperationException("aggregateId should not be null");
            }
            context.Aggregate = await Session.LoadAggregate<TAggregate>(aggregateId);

            var userId = _userIdSelector(context.Command);
            if (userId != null)
            {
                context.User = await Session.LoadUserAsync(userId);
            }
        }

        protected virtual Task AdditionalCommand(TContext context)
        {
            return Task.CompletedTask;
        }

        private void HandleCommand(TContext context)
        {
            AggregateInvoker.HandleCommand(context.Aggregate, context.User, context.Command);
        }

        protected virtual void StoreAggregate(TContext context)
        {
            Session.StoreChanges(context.Aggregate);
        }
    }
}