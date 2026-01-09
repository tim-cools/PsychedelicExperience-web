using System;
using System.Threading.Tasks;
using Marten;

namespace PsychedelicExperience.Common.Messages
{
    public abstract class QueryHandler<TCommand, TResult> : IAsyncRequestHandler<TCommand, TResult> 
        where TCommand : IRequest<TResult>
    {
        protected IQuerySession Session { get; }

        protected QueryHandler(IQuerySession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            Session = session;
        }

        public async Task<TResult> Handle(TCommand query)
        {
            return await Execute(query);
        }

        protected abstract Task<TResult> Execute(TCommand query);
    }
}