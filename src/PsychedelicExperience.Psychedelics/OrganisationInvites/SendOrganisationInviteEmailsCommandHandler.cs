using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserContactLog;
using PsychedelicExperience.Psychedelics.Messages.OrganisationInvites.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.OrganisationView;
using PsychedelicExperience.Psychedelics.Security;
using Contact = PsychedelicExperience.Psychedelics.OrganisationView.Contact;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationInvites
{
    public class SendOrganisationInviteEmailsCommandHandler : CommandHandler<SendOrganisationInviteEmailsCommand, ContentResult>
    {
        private readonly IMailSender _mailSender;
        private readonly IProtectedRequestProvider _protectedRequestProvider;
        private readonly IUserContactLogger _logger;
        private readonly IConfiguration _configuration;

        public SendOrganisationInviteEmailsCommandHandler(IDocumentSession session, IMailSender mailSender, IConfiguration configuration, IProtectedRequestProvider protectedRequestProvider, IUserContactLogger logger)
            : base(session)
        {
            _mailSender = mailSender;
            _configuration = configuration;
            _protectedRequestProvider = protectedRequestProvider;
            _logger = logger;
        }

        protected override async Task<ContentResult> Execute(SendOrganisationInviteEmailsCommand command)
        {
            var currentUser = await Session.LoadUserAsync(command.CurrentUserId);
            if (currentUser == null)
            {
                return ContentResult.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Current User not found.");
            }

            if (!currentUser.IsAdministrator())
            {
                return ContentResult.Failed("CurrentUserId", ErrorCodes.UserNotFound, "Not allowed.");
            }

            var organisations = await GetAllOrganisations();
            var result = new StringBuilder();

            foreach (var organisation in organisations.Where(organisation => organisation.Owners.Count == 0))
            {
                foreach (var owner in organisation.Contacts.Where(contact => contact.Type == ContactTypes.EMail))
                {
                    await Invite(currentUser, command, owner, result, organisation);
                }
            }

            return new ContentResult(true, result.ToString());
        }

        private async Task Invite(User currentUser, SendOrganisationInviteEmailsCommand command, Contact owner, StringBuilder result, Organisation organisation)
        {
            if (command.Filter != null &&
                !string.Equals(owner.Value, command.Filter, StringComparison.OrdinalIgnoreCase))
            {
                result.AppendLine("skipped: " + organisation.Name + " > " + owner.Value);
                return;
            }

            var request = await SendEmail(owner, organisation);

            await UpdateAggregate(currentUser, command.CurrentUserId, owner, organisation);

            result.AppendLine("sent: " + organisation.Name + " > " + owner.Value);

            _logger.Log(command.CurrentUserId, new EMail(owner.Value), nameof(Organisation), organisation.Id, nameof(Invite), request);
        }

        private async Task<string> SendEmail(Contact owner, Organisation organisation)
        {
            var request = await _protectedRequestProvider.GenerateInvite(organisation.Id, owner.Value);
            await _mailSender.SendInviteOrganisationEmail(_configuration, organisation, owner, request);
            return request;
        }

        private async Task UpdateAggregate(User currentUser, UserId currentUserId, Contact owner,
            Organisation organisation)
        {
            var aggregate = await Session.LoadAggregate<Organisations.Organisation>(organisation.Id);
            var invitOrganisationOwner = new InvitOrganisationOwner(new OrganisationId(organisation.Id), currentUserId,
                new EMail(owner.Value));
            aggregate.Handle(currentUser, invitOrganisationOwner);

            Session.StoreChanges(aggregate);
        }

        private async Task<IReadOnlyList<Organisation>> GetAllOrganisations()
        {
            return await Session.Query<Organisation>()
                .Where(organisation => !organisation.Removed)
                .ToListAsync();
        }
    }

    public class VerifyOrganisationInviteHandler : CommandHandler<VerifyOrganisationInvite, VerifyOrganisationInviteResult>
    {
        private readonly IProtectedRequestProvider _protectedRequestProvider;

        public VerifyOrganisationInviteHandler(IDocumentSession session, IProtectedRequestProvider protectedRequestProvider)
            : base(session)
        {
            _protectedRequestProvider = protectedRequestProvider;
        }

        protected override async Task<VerifyOrganisationInviteResult> Execute(VerifyOrganisationInvite command)
        {
            var currentUser = command.CurrentUserId != null ? await Session.LoadUserAsync(command.CurrentUserId) : null;
            var result = await _protectedRequestProvider.ValidateInvite(command.Token, currentUser?.Email);
            if (!result.Success())
            {
                return new VerifyOrganisationInviteResult(result.Key, result.OrganisationId);
            }

            var organisation = await Session.LoadAsync<Organisation>(result.OrganisationId.Value);
            if (organisation == null)
            {
                return new VerifyOrganisationInviteResult("organisation-not-found");
            }

            if (organisation.IsOwner(currentUser))
            {
                return new VerifyOrganisationInviteResult("user-already-owner", organisation.Id);
            }

            return new VerifyOrganisationInviteResult(true, organisation.Id);
        }
    }

    public class ConfirmOrganisationInviteHandler : CommandHandler<ConfirmOrganisationInvite, ConfirmOrganisationInviteResult>
    {
        private readonly IProtectedRequestProvider _protectedRequestProvider;

        public ConfirmOrganisationInviteHandler(IDocumentSession session, IProtectedRequestProvider protectedRequestProvider)
            : base(session)
        {
            _protectedRequestProvider = protectedRequestProvider;
        }

        protected override async Task<ConfirmOrganisationInviteResult> Execute(ConfirmOrganisationInvite command)
        {
            var currentUser = await Session.LoadUserAsync(command.CurrentUserId);
            var result = await _protectedRequestProvider.ValidateInvite(command.Token, currentUser?.Email);
            if (!result.Success())
            {
                return new ConfirmOrganisationInviteResult(false, result.Key);
            }

            await UpdateAggregate(result.OrganisationId.Value, currentUser);

            return new ConfirmOrganisationInviteResult(true);
        }


        private async Task UpdateAggregate(Guid id, User currentUser)
        {
            var aggregate = await Session.LoadAggregate<Organisations.Organisation>(id);
            var confirmOrganisationOwner = new ConfirmOrganisationOwner(new OrganisationId(id), (UserId)currentUser.Id);
            aggregate.Handle(currentUser, confirmOrganisationOwner);

            Session.StoreChanges(aggregate);
        }
    }
}