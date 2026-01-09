using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElasticEmailClient;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace PsychedelicExperience.Common.Mail
{
    public interface IMailSender
    {
        Task SendMail(MailboxAddress toAddress, Template template, string replyToAddress = null);
        Task SendMail(MailboxAddress toAddress, string subject, string body);
        Task SendMail(string subject, string body);
    }

    public abstract class Template
    {
        public abstract string TemplateName {get;}
        public abstract string FromAddressPrefix { get; }

        public abstract string GetSubject();
        public abstract Dictionary<string, string> GetMergeFields();
    }

    public class MailSender : IMailSender
    {
        private readonly MailOptions _options;
        private readonly ILogger<MailSender> _logger;

        public MailSender(ILogger<MailSender> logger, IOptions<MailOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value;
        }

        public Task SendMail(MailboxAddress toAddress, Template template, string replyToAddress = null)
        {
            if (toAddress == null) throw new ArgumentNullException(nameof(toAddress));
            if (template == null) throw new ArgumentNullException(nameof(template));

            _logger.LogTraceMethod(nameof(SendMail), new { template.TemplateName, to = toAddress.Address });

            var fromAddress = FromAddress(template);

            Api.Email.Send(
                to: new List<string>{ toAddress.Address },
                fromName: fromAddress.Name,
                from: fromAddress.Address,
                subject: template.GetSubject(),
                template: template.TemplateName,
                merge: template.GetMergeFields(),
                replyTo: replyToAddress
            );

            _logger.LogInformationMethod(nameof(SendMail), new { template.TemplateName, from = fromAddress.Address, to = toAddress.Address });

            return Task.CompletedTask;
        }

        public async Task SendMail(MailboxAddress toAddress, string subject, string body)
        {
            if (toAddress == null) throw new ArgumentNullException(nameof(toAddress));

            _logger.LogTraceMethod(nameof(SendMail), new { subject, body });

            var fromAddress = FromAddress();
            var bccAddress = new MailboxAddress(_options.AdminEmailName, _options.AdminEmailAddress);

            using (var smtp = CreateSmtpClient())
            {
                var message = CreateMessage(fromAddress, toAddress, subject, body, bccMailboxAddress: bccAddress);

                await RetrySend(smtp, message);

                smtp.Disconnect(true);
            }

            _logger.LogInformationMethod(nameof(SendMail), new { subject, body, admin = _options.AdminEmailAddress, to = toAddress.Address });
        }

        public async Task SendMail(string subject, string body)
        {
            _logger.LogTraceMethod(nameof(SendMail), new { subject, body });

            var fromAddress = FromAddress();
            var toAddress = new MailboxAddress(_options.AdminEmailName, _options.AdminEmailAddress);

            using (var smtp = CreateSmtpClient())
            {
                var message = CreateMessage(fromAddress, toAddress, subject, body);

                await RetrySend(smtp, message);

                await smtp.DisconnectAsync(true);
            }

            _logger.LogInformationMethod(nameof(SendMail), new { subject, body, admin = _options.AdminEmailAddress, to = toAddress.Address });
        }

        private MailboxAddress FromAddress(Template template = null)
        {
            var prefix = template?.FromAddressPrefix != null 
                ? $"{template?.FromAddressPrefix} via " 
                : string.Empty;

            return new MailboxAddress(prefix + _options.FromEmailName, _options.FromEmailAddress);
        }

        private async Task RetrySend(SmtpClient smtp, MimeMessage message)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    await smtp.SendAsync(message);

                    return;
                }
                catch (Exception exception)
                {
                    if (count++ >= 3)
                    {
                        _logger.LogErrorMethod(nameof(RetrySend), exception, new
                        {
                            subject = message.Subject,
                            body = message.Body,
                            to = message.To
                        });
                        throw;
                    }
                    _logger.LogTraceMethod(nameof(RetrySend), exception, new
                    {
                        subject = message.Subject,
                        body = message.Body,
                        to = message.To,
                        count
                    });
                }
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpClient = new SmtpClient { Timeout = 20000 };
            smtpClient.Connect(_options.SmtpServer, _options.SmtpPort);

            Authenticate(smtpClient);

            return smtpClient;
        }

        private void Authenticate(SmtpClient smtpClient)
        {
            if (string.IsNullOrEmpty(_options.SmtpUser) || string.IsNullOrEmpty(_options.SmtpPassword)) return;

            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
            smtpClient.Authenticate(_options.SmtpUser, _options.SmtpPassword);
        }

        private static MimeMessage CreateMessage(MailboxAddress fromAddress, MailboxAddress toAddress, string subject, string body, string html = null, MailboxAddress bccMailboxAddress = null)
        {
            var builder = new BodyBuilder
            {
                TextBody = body,
                HtmlBody = html
            };
           
            var message = new MimeMessage
            {
                From = {fromAddress},
                To = {toAddress},
                Subject = subject,
                Body = builder.ToMessageBody()
            };

            if (bccMailboxAddress != null)
            {
                message.Bcc.Add(bccMailboxAddress);
            }

            return message;
        }
    }

    public class MailOptions
    {
        public string AdminEmailName { get; set; }
        public string AdminEmailAddress { get; set; }

        public string FromEmailName { get; set; }
        public string FromEmailAddress { get; set; }

        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpUser { get; set; }
    }
}