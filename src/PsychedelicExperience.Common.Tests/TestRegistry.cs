using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.StructureMap;
using PsychedelicExperience.Common.Tests.Storage;
using StructureMap;

namespace PsychedelicExperience.Common.Tests
{
    public class TestRegistry : Registry
    {
        public TestRegistry()
        {
            Scan(options =>
            {
                options.TheCallingAssembly();
                options.IncludeNamespaceContainingType<TestStoreDatabaseFactory>();
                options.Convention<AllInterfacesConvention>();
            });

            For<IOptions<MailOptions>>()
                .Use(new OptionsManager<MailOptions>(new OptionsFactory<MailOptions>(new IConfigureOptions<MailOptions>[]{ }, new IPostConfigureOptions<MailOptions>[]{})))
                .Singleton();

            For<IContext>().Use("Return context", context => context);

            For<IServiceProvider>().Use<ServiceProvider>();
            For<ILoggerFactory>().Use<DummyLoggerFactory>();
            For(typeof(ILogger<>)).Use(typeof(DummyLogger<>));
            For<IMailSender>().Use<DummyMailSender>();
            For<ISendInBlue>().Use<DummySendInBlue>();
        }
    }

    public class DummySendInBlue : ISendInBlue
    {
        public Task AddNewsletter(string email, string name) => Task.CompletedTask;
    }

    public class DummyMailSender : IMailSender
    {
        public Task SendMail(MailboxAddress toAddress, Template template, string replyToAddress = null)
        {
            return Task.CompletedTask;
        }

        public Task SendMail(MailboxAddress toAddress, string subject, string body)
        {
            return Task.CompletedTask;
        }

        public Task SendMail(string subject, string body)
        {
            return Task.CompletedTask;
        }
    }

    public class DummyLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DummyLogger();
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}