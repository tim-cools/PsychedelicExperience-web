using System;
using System.Threading.Tasks;
using Marten;

namespace PsychedelicExperience.Common.Tests
{
    public abstract class ServiceTestBase<T>
    {
        private readonly IntegrationTestFixture _fixture;

        protected ServiceTestBase(IntegrationTestFixture fixture)
        {
            _fixture = fixture;

            SessionScope<IDocumentStore>(CleanDocuments);

            SessionScope<T>(PreGiven);
            SessionScope<T>(Given);
            SessionScope<T>(When);
        }

        protected virtual void PreGiven(TestContext<T> context)
        {
        }

        protected virtual void Given(TestContext<T> context)
        {
        }

        protected abstract void When(TestContext<T> context);

        protected void SessionScope(Action<TestContext<T>> execute)
        {
            SessionScope<T>(execute);
        }

        protected TResult SessionScopeResult<TResult>(Func<TestContext<T>, TResult> execute)
        {
            var result = default(TResult); ;
            SessionScope<T>(context => result = execute(context));
            return result;
        }

        protected TResult SessionScopeResult<TResult>(Func<TestContext<T>, Task<TResult>> execute)
        {
            var result = default(TResult); ;
            SessionScope<T>(context => result = execute(context).GetResult());
            return result;
        }

        protected void SessionScope<TService>(Action<TestContext<TService>> execute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            using (var container = _fixture.OpenContainerScope())
            {
                var context = new TestContext<TService>(container);
                execute(context);
            }
        }

        private void CleanDocuments(TestContext<IDocumentStore> context)
        {
            context.Service.Advanced.Clean.DeleteAllDocuments();
        }
    }
}
