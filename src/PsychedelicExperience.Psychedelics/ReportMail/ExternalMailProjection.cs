using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;

namespace PsychedelicExperience.Psychedelics.ReportMail
{
    public class ExternalMailSent_v3
    {
        public Guid Id { get; set; }
    }

    public class ExternalMailUserContact
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ExternalMailOrganisationContact
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Emails { get; set; } = new List<string>();
        public List<Guid> Owners { get; set; } = new List<Guid>();
    }

    public class ExternalMailProjection : EventProjection
    {
        private readonly IMailSender _mailSender;
        private readonly ILogger<ExternalMailProjection> _logger;
        private readonly IConfiguration _configuration;

        public override Type ViewType => typeof(ExternalMailSent_v3);

        public ExternalMailProjection(IConfiguration configuration, IMailSender mailSender, ILogger<ExternalMailProjection> logger)
        {
            _configuration = configuration;
            _mailSender = mailSender;
            _logger = logger;

            if (!configuration.SendMailEnabled())
            {
                _logger.LogWarningMethod(nameof(ExternalMailProjection), new { skipped = true });
                return;
            }

            EventAsync(WithLogging<OrganisationAdded>(OrganisationAdded));
            EventAsync(WithLogging<OrganisationNameChanged>(OrganisationNameChanged));
            EventAsync(WithLogging<OrganisationContactAddded>(OrganisationContactAddded));
            EventAsync(WithLogging<OrganisationContactRemoved>(OrganisationContactRemoved));
            EventAsync(WithLogging<OrganisationContacted>(OrganisationContacted));
            EventAsync(WithLogging<OrganisationRemoved>(OrganisationRemoved));
            EventAsync(WithLogging<OrganisationOwnerAdded>(OrganisationOwnerAdded));
            EventAsync(WithLogging<OrganisationOwnerRemoved>(OrganisationOwnerRemoved));

            EventAsync(WithLogging<UserProfileCreated>(UserProfileCreated));
            EventAsync(WithLogging<UserProfileDisplayNameChanged>(UserProfileDisplayNameChanged));
            EventAsync(WithLogging<UserProfileEMailChanged>(UserProfileEMailChanged));
        }

        private Func<IDocumentSession, Guid, T, Task> WithLogging<T>(Func<IDocumentSession, Guid, T, Task> action)
        {
            return async (session, id, @event) =>
            {
                var type = typeof(T);
                var json = JsonConvert.SerializeObject(@event);

                _logger.LogInformationMethod(nameof(WithLogging), new { type, json });

                try
                {
                    await action(session, id, @event);

                    _logger.LogInformationMethod(nameof(WithLogging), new { json, finished = true });
                }
                catch (Exception exception)
                {
                    _logger.LogErrorMethod(nameof(WithLogging), exception, new { type, json });

                    var message = $"Processing event failed: {json}";
                    throw new InvalidOperationException(message, exception);
                }
            };
        }

        private async Task OrganisationAdded(IDocumentSession session, Guid id, OrganisationAdded @event)
        {
            var organisation = new ExternalMailOrganisationContact
            {
                Id = id,
                Name = (string) @event.Name,
                Emails = GetEmails(@event)
            };

            session.Store(organisation);

            if (IsOld(@event)) return;

            await SendOrganisationAddedEmails(session, @event, organisation);
        }

        private async Task SendOrganisationAddedEmails(IDocumentSession session, OrganisationAdded @event,
            ExternalMailOrganisationContact organisation)
        {
            var user = await LoadUser(session, @event.UserId, false);
            if (user == null) return;

            await SendOrganisationAddedEmail(false, @event.OrganisationId, (string) @event.Name, user.Email);

            foreach (var email in organisation.Emails)
            {
                if (email != user.Email)
                {
                    await SendOrganisationAddedEmail(true, @event.OrganisationId, (string) @event.Name, email);
                }
            }
        }

        private async Task<ExternalMailUserContact> LoadUser(IDocumentSession session, UserId userId, bool optional)
        {
            var user = await session.LoadAsync<ExternalMailUserContact>((Guid) userId);
            if (user == null)
            {
                if (!optional)
                {
                    throw new InvalidOperationException("Could not load user: " + userId);
                }
                _logger.LogWarningMethod(nameof(LoadUser), new { message = "Could not load user: " + userId });
            }
            return user;
        }

        private static List<string> GetEmails(OrganisationAdded @event)
        {
            var emails = @event.Contacts != null 
                ? @event.Contacts
                    .Where(contact => contact.Type == ContactTypes.EMail)
                    .Select(contact => contact.Value.ToLowerInvariant())
                    .ToList()
                : new List<string>();

            if (@event.EMail != null)
            {
                emails.Add(((string) @event.EMail).ToLowerInvariant());
            }

            return emails;
        }

        private async Task SendOrganisationAddedEmail(bool isOwner, OrganisationId organisationId, string name, string email)
        {
            var address = new MailboxAddress(email);

            var url = _configuration.WebUrl(
                $"/organisation/{WebUtility.UrlDecode(new ShortGuid((Guid) organisationId))}");

            var template = new OrganisationAddedTemplate(isOwner, name, url);

            await _mailSender.SendMail(address, template);
        }

        private async Task OrganisationContacted(IDocumentSession session, Guid id, OrganisationContacted @event)
        {
            if (IsOld(@event)) return;

            var organisation = await LoadOrganisation(session, @event.OrganisationId);

            var emails = organisation.Owners == null || organisation.Owners.Count == 0
                ? organisation.Emails
                : GetEmails(session ,organisation.Owners);

            foreach (var email in emails)
            {
                await SendOrganisationContactedEmail(organisation, @event, email);
            }
        }

        private static bool IsOld(Event @event)
        {
            return @event.EventTimestamp < DateTime.Today.AddDays(-3);
        }

        private List<string> GetEmails(IDocumentSession session, List<Guid> userIds)
        {
            var users = session.LoadMany<ExternalMailUserContact>(userIds.ToArray());
            return users.Select(user => user.Email).ToList();
        }

        private async Task SendOrganisationContactedEmail(ExternalMailOrganisationContact organisation, OrganisationContacted @event, string email)
        {
            var address = new MailboxAddress(email);

            var url = _configuration.WebUrl(
                $"/organisation/{WebUtility.UrlDecode(new ShortGuid((Guid) @event.OrganisationId))}");

            var template = new OrganisationContactedTemplate(
                organisation.Name,
                url,
                @event.UserName,
                @event.Subject,
                @event.EMail,
                @event.Message);

            await _mailSender.SendMail(address, template, @event.EMail);
        }

        private async Task OrganisationNameChanged(IDocumentSession session, Guid id, OrganisationNameChanged @event)
        {
            var organisation = await LoadOrganisation(session, @event.OrganisationId);

            organisation.Name = (string) @event.Name;

            session.Store(organisation);
        }

        private async Task OrganisationContactAddded(IDocumentSession session, Guid id, OrganisationContactAddded @event)
        {
            if (@event.Type != ContactTypes.EMail) return;
            
            var organisation = await LoadOrganisation(session, @event.OrganisationId);
            var email = @event.Value.ToLowerInvariant();
            
            organisation.Emails.Add(email);

            session.Store(organisation);
            
            await SendOrganisationAddedEmail(true, @event.OrganisationId, organisation.Name, email);
        }

        private static async Task<ExternalMailOrganisationContact> LoadOrganisation(IDocumentSession session, OrganisationId organisationId)
        {
            var organisation = await session.LoadAsync<ExternalMailOrganisationContact>((Guid) organisationId);
            if (organisation == null)
            {
                throw new InvalidOperationException("Could not load organisation: " + organisationId);
            }
            if (organisation.Owners == null)
            {
                organisation.Owners = new List<Guid>();
            }
            if (organisation.Emails == null)
            {
                organisation.Emails = new List<string>();
            }
            return organisation;
        }

        private async Task OrganisationContactRemoved(IDocumentSession session, Guid id, OrganisationContactRemoved @event)
        {
            if (@event.Type != ContactTypes.EMail) return;

            var organisation = await LoadOrganisation(session, @event.OrganisationId);

            organisation.Emails.Remove(@event.Value.ToLowerInvariant());

            session.Store(organisation);
        }

        private Task OrganisationRemoved(IDocumentSession session, Guid id, OrganisationRemoved @event)
        {
            session.Delete<ExternalMailOrganisationContact>((Guid)@event.OrganisationId);
            
            return Task.CompletedTask;
        }

        private async Task OrganisationOwnerAdded(IDocumentSession session, Guid id, OrganisationOwnerAdded @event)
        {
            var organisation = await LoadOrganisation(session, @event.OrganisationId);

            organisation.Owners.Add((Guid)@event.OwnerId);

            session.Store(organisation);

            if (IsOld(@event)) return;
          
            var user = await LoadUser(session, @event.OwnerId, true);

            if (user != null)
            {
                await SendOrganisationOwnerAddedEmail(organisation, user);
            }
        }

        private async Task SendOrganisationOwnerAddedEmail(ExternalMailOrganisationContact organisation, ExternalMailUserContact user)
        {
            var url = _configuration.WebUrl(
                $"/organisation/{WebUtility.UrlDecode(new ShortGuid(organisation.Id))}");

            var address = new MailboxAddress(user.Email);
            var template = new OrganisationOwnerAddedTemplate(organisation.Name, url);

            await _mailSender.SendMail(address, template);
        }

        private async Task OrganisationOwnerRemoved(IDocumentSession session, Guid id, OrganisationOwnerRemoved @event)
        {
            var organisation = await LoadOrganisation(session, @event.OrganisationId);

            organisation.Owners.Remove((Guid) @event.OwnerId);

            session.Store(organisation);
        }

        private Task UserProfileCreated(IDocumentSession session, Guid id, UserProfileCreated @event)
        {
            var user = new ExternalMailUserContact
            {
                Id = (Guid) @event.UserProfileId,
                Name = (string) @event.DisplayName,
                Email = ((string) @event.EMail).ToLowerInvariant()
            };

            session.Store(user);
            
            return Task.CompletedTask;
        }

        private async Task UserProfileDisplayNameChanged(IDocumentSession session, Guid id, UserProfileDisplayNameChanged @event)
        {
            var user = await LoadUser(session, @event.UserId, false);

            user.Name = (string) @event.DisplayName;

            session.Store(user);
        }

        private async Task UserProfileEMailChanged(IDocumentSession session, Guid id, UserProfileEMailChanged @event)
        {
            var user = await LoadUser(session, @event.UserId, false);

            user.Email = ((string)@event.EMail).ToLowerInvariant();

            session.Store(user);
        }

        /* private async Task ExperienceAdded(IDocumentSession session, Guid id, ExperienceAdded @event)
        {
            if (IsOld(@event)) return;

            var user = await LoadUser(session, @event.UserId, true);
            if (user == null) return;

            await SendExperienceAddedEmail(user);
        }
        */

        private async Task SendExperienceAddedEmail(ExternalMailUserContact user)
        {
            var address = new MailboxAddress(user.Email);
            var template = new ExperienceAddedTemplate(user.Name);

            await _mailSender.SendMail(address, template);
        }
    }

    internal class OrganisationContactedTemplate : Template
    {
        private readonly string _organisationName;
        private readonly string _url;
        private readonly string _user;
        private readonly string _subject;
        private readonly string _email;
        private readonly string _message;

        public override string TemplateName => "20170721-organisation-contacted";
        public override string FromAddressPrefix => null;

        public OrganisationContactedTemplate(string organisationName, string url, string user, string subject, string email, string message)
        {
            _organisationName = organisationName;
            _url = url;
            _user = user;
            _subject = subject;
            _email = email;
            _message = message;
        }

        public override string GetSubject()
        {
            return _user != null 
                ? _user + ", contacted you via PsychedelicExperience"
                : "A user contacted you via PsychedelicExperience";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            var userCaption = _user != null
                ? _user + ", a registered user from our platform"
                : "A non registered user";

            return new Dictionary<string, string>
            {
                { "organisationName", _organisationName },
                { "user", userCaption },
                { "url", _url },
                { "subject", _subject },
                { "email", _email },
                { "message", _message }
            };
        }
    }

    internal class OrganisationAddedTemplate : Template
    {
        private readonly string _organisationName;
        private readonly string _url;
        private readonly string _title;

        public override string TemplateName => "20170721-organisation-added";
        public override string FromAddressPrefix => null;

        public OrganisationAddedTemplate(bool owner, string organisationName, string url)
        {
            _title = owner
                ? $"One of our users added a new organisation to our organisations directory: {organisationName}"
                : $"Thank you for adding a new organisation: {organisationName}";

            _organisationName = organisationName;
            _url = url;
        }

        public override string GetSubject()
        {
            return _organisationName + " has been added to PsychedelicExperience.net";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            return new Dictionary<string, string>
            {
                { "title", _title },
                { "organisationName", _organisationName },
                { "url", _url }
            };
        }
    }

    internal class OrganisationOwnerAddedTemplate : Template
    {
        private readonly string _organisationName;
        private readonly string _url;

        public override string TemplateName => "20170721-organisation-owner-added";
        public override string FromAddressPrefix => null;

        public OrganisationOwnerAddedTemplate(string organisationName, string url)
        {
            _organisationName = organisationName;
            _url = url;
        }

        public override string GetSubject()
        {
            return $"You are now owner of {_organisationName} on Psychedelic Experience";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            return new Dictionary<string, string>
            {
                { "organisationName", _organisationName },
                { "url", _url }
            };
        }
    }

    internal class ExperienceAddedTemplate : Template
    {
        private readonly string _user;

        public ExperienceAddedTemplate(string user)
        {
            _user = user;
        }

        public override string TemplateName => "20170721-experience-added";
        public override string FromAddressPrefix => null;

        public override string GetSubject()
        {
            return "Thank you for sharing your experience!";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            return new Dictionary<string, string>
            {
                { "user", _user}
            };
        }
    }
}