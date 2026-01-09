using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;

namespace PsychedelicExperience.Common.Messages
{
    public abstract class CommandHandler<TCommand, TResult> : IAsyncRequestHandler<TCommand, TResult>
        where TCommand : IRequest<TResult>
        where TResult : Result, new()
    {
        protected IDocumentSession Session { get; }

        protected CommandHandler(IDocumentSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<TResult> Handle(TCommand query)
        {
            try
            {
                var result = await Execute(query);
                if (result.Succeeded)
                {
                    await Session.SaveChangesAsync();
                }
                return result;
            }
            catch (BusinessException businessException)
            {
                return new TResult { Succeeded = false, Errors = businessException.Errors };
            }
#pragma warning disable CS0168
#if DEBUG
            catch (Exception exception)
            {
                //bad practice, but this is here to put in a breakpoint 
                //to be able to break on exceptions without have the break on CLR exceptions enabled
                throw;
            }
#endif
#pragma warning restore CS0168            
        }

        protected abstract Task<TResult> Execute(TCommand command);
    }

    public abstract class CommandHandler<TCommand> : CommandHandler<TCommand, Result> 
        where TCommand : IRequest<Result>
    {
        protected CommandHandler(IDocumentSession session) : base(session)
        {
        }
    }
}