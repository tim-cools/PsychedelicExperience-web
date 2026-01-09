using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace PsychedelicExperience.Common.Mail
{
    public interface ISendInBlue
    {
        Task AddNewsletter(string email, string name);
    }

    public class SendInBlue : ISendInBlue
    {
        private readonly MailOptions _options;
        private readonly ILogger<MailSender> _logger;
        private readonly IConfiguration _configuration;

        public SendInBlue(ILogger<MailSender> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (_configuration.SendInBlueApiEnabled())
            {
                Configuration.Default.AddApiKey("api-key", _configuration.SendInBlueApi());
            }
        }

        public async Task AddNewsletter(string email, string name)
        {
            if (!_configuration.SendInBlueApiEnabled())
            {
                throw new InvalidOperationException("SendInBlueApi not defined");
            }

            var listId = _configuration.SendInBlueNewsletterListId();
            var apiInstance = new ContactsApi();

            var contact = await GetContact(email, apiInstance);

            if (contact == null)
            {
                await Create(email, name, apiInstance, listId);
            }
            else if (contact.ListIds?.Contains(listId) != true)
            {
                await apiInstance.AddContactToListAsync(listId, new AddContactToList(new List<string> {email}));
                _logger.LogDebug($"Newsletter contact existing, added to list: {name} ({listId}))");
            }
            else
            {
                _logger.LogDebug($"Newsletter contact existing in list: {name} ({listId}))");
            }
        }

        private static async Task<GetExtendedContactDetails> GetContact(string email, ContactsApi apiInstance)
        {
            try
            {
                return await apiInstance.GetContactInfoAsync(email);
            }
            catch (ApiException e)
            {
                if (e.ErrorCode == 404) return null;

                throw;
            }
        }

        private async Task Create(string email, string name, ContactsApi apiInstance, long? listId)
        {
            var createContact = new CreateContact();
            createContact.Email = email;
            createContact.Attributes = new { FNAME = name };
            createContact.ListIds = new List<long?> {listId};

            var result = await apiInstance.CreateContactAsync(createContact);
            _logger.LogDebug($"Newsletter contact added: {name} ({listId}))");
        }
    }

    public class SendInBlueOptions
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