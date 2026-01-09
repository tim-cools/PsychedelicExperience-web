using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Email;

namespace PsychedelicExperience.Web.Infrastructure
{
    public static class LoggingConfiguration
    {
        const string Template = "{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception}";
        const int RetainedFileCountLimit = 124;
     
        public static Action<ILoggingBuilder> Configure(IConfiguration configuration, IHostingEnvironment env) 
            => builder => Configure(builder, configuration, env);

        private static void Configure(ILoggingBuilder builder, IConfiguration configuration, IHostingEnvironment env)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (env == null) throw new ArgumentNullException(nameof(env));

            var isMarten = Matching.FromSource("Marten");
            var isFramework = Matching.FromSource("Microsoft").Or(Matching.FromSource("AspNet"));
            var isRequest = Matching.FromSource("PsychedelicExperience.Web.Infrastructure.LogRequestMiddleware");
            var isError = Level(LogEventLevel.Warning).Or(Level(LogEventLevel.Error)).Or(Level(LogEventLevel.Fatal));

            var logFolder = configuration.LoggingFolder();

            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Filter.With(new ProductionFilter())
                .MinimumLevel.Debug()
                .WithLogger(logFolder, "marten.log", isMarten)
                .WithLogger(logFolder, "framework.log", isFramework)
                .WithLogger(logFolder, "errors.log", isError)
                .WithLogger(logFolder, "requests.log", isRequest)
                .WithLoggerExcluded(logFolder, "pex.log", isMarten, isFramework, isRequest);
            ConfigureEmails(loggerConfig, configuration);

            var logger = loggerConfig.CreateLogger();

            builder.AddSerilog(logger);

            if (env.IsDevelopment())
            {
                builder.AddConsole();
                builder.AddDebug();
            }
        }

        private static void ConfigureEmails(LoggerConfiguration configuration, IConfiguration config)
        {
            var isError = Level(LogEventLevel.Warning)
                .Or(Level(LogEventLevel.Error))
                .Or(Level(LogEventLevel.Fatal));

            var smtpUser = config.MailOptionsSmtpUser();
            var credentialsByHost = !string.IsNullOrWhiteSpace(smtpUser) ? new NetworkCredential
            {
                Password = config.MailOptionsSmtpPassword(),
                UserName = smtpUser
            } : null;

            var email = config.MailOptionsAdminEmailAddress();
            var settings = new EmailConnectionInfo
            {
                EmailSubject = $"[pex-api-{config.Environment()}] {{Timestamp}} [{{Level}}] occurred",
                ToEmail = email,
                FromEmail = config.MailOptionsFromEmailAddress(),
                NetworkCredentials = credentialsByHost,
                MailServer = config.MailOptionsSmtpServer(),
                Port = config.MailOptionsSmtpPort()
            };

            configuration
                .Filter.ByIncludingOnly(isError)
                .WriteTo.Email(settings);
        }
        private static LoggerConfiguration WithLogger(this LoggerConfiguration configuration, string logFolder, string fileName, Func<LogEvent, bool> filter)
        {
            return configuration.WriteTo.Logger(config => config
                .Filter.ByIncludingOnly(filter)
                .WriteTo.RollingFile(Path.Combine(logFolder, fileName), retainedFileCountLimit: RetainedFileCountLimit,
                    outputTemplate: Template));
        }

        private static LoggerConfiguration WithLoggerExcluded(this LoggerConfiguration configuration, string logFolder, string fileName, params Func<LogEvent, bool>[] filters)
        {
            var filter = Or(filters);

            return configuration.WriteTo.Logger(config => config
                .Filter.ByExcluding(filter)
                .WriteTo.RollingFile(Path.Combine(logFolder, fileName), retainedFileCountLimit: RetainedFileCountLimit,
                    outputTemplate: Template));
        }

        [DebuggerStepThrough]
        private static Func<LogEvent, bool> Or(Func<LogEvent, bool>[] filters)
        {
            return filters.Aggregate<Func<LogEvent, bool>, Func<LogEvent, bool>>(null, (current, func) => current == null ? func : Or(current, func));
        }

        [DebuggerStepThrough]
        private static Func<LogEvent, bool> Or(this Func<LogEvent, bool> first, Func<LogEvent, bool> second)
        {
            return @event => first(@event) || second(@event);
        }

        [DebuggerStepThrough]
        private static Func<LogEvent, bool> Level(LogEventLevel level)
        {
            return @event => @event.Level == level;
        }
    }

    public class ProductionFilter : ILogEventFilter
    {
        public bool IsEnabled(LogEvent logEvent)
        {
            return logEvent.Level switch
            {
                LogEventLevel.Warning => FilterWarning(logEvent),
                _ => true
            };
        }

        private static bool FilterWarning(LogEvent logEvent)
        {
            var sourceContext = GetSource(logEvent);
            if (sourceContext?.EndsWith("OpenIdConnectServerHandler") != true) return true;

            //If used behind a load balancer (aws) then AllowInsecureHttp should be set to false in production
            //see: https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server/blob/398c2ea282d671c4b160440f6c059248cf5cee0d/src/AspNet.Security.OpenIdConnect.Server/OpenIdConnectServerInitializer.cs#L174
            return !logEvent.MessageTemplate.Text.StartsWith("Disabling the transport security requirement");
        }

        private static string GetSource(LogEvent logEvent)
        {
            if (!logEvent.Properties.TryGetValue("SourceContext", out var value))
            {
                return null;
            }

            var scalar = value as ScalarValue;

            return scalar?.Value?.ToString();
        }
    }
}