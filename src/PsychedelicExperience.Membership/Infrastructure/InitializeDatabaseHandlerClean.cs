using System;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Clients.Domain;
using PsychedelicExperience.Membership.Messages.Infrastructure;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using Shouldly;
using Role = PsychedelicExperience.Membership.Messages.Users.Role;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.Infrastructure
{
    public class InitializeDatabaseHandler : CommandHandler<InitializeDatabaseCommand>
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;

        public InitializeDatabaseHandler(IDocumentSession session, UserManager<User> userManager, IConfiguration configuration, IMediator mediator)
            : base(session)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mediator = mediator;
        }

        protected override async Task<Result> Execute(InitializeDatabaseCommand command)
        {
            Session
                .M20170103_MigrateOldUsers();

            UpdateClients();

            await InitializeUsers();

            return Result.Success;
        }

        private async Task InitializeUsers()
        {
            await InitializeUser("tim@dummy.com", "Tim Psy Ex", "Aa-123456");
            
            if (_configuration.Environment() == "development")
            {
                await InitializeUser(_configuration.ApiTestAccountEMail(), "Automated Test Admin Account", _configuration.ApiTestAccountPassword());
            }
        }

        private async Task InitializeUser(string eMail, string displayName, string password)
        {
            var user = await _userManager.FindByEmailAsync(eMail);
            if (user == null)
            {
                user = await RegisterUser(eMail, displayName, password);
            }
            else
            {
                await UpdateUser(displayName, password, user);
            }

            if (!user.IsAdministrator())
            {
                var command = new AddUserToRole(null, (UserId) user.Id, Role.Administrator, true);
                await _mediator.Send(command);
            }
        }

        private async Task UpdateUser(string displayName, string password, User user)
        {
            var tokenResult = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, tokenResult, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not Reset Password: " + user.Email);
            }

            user.DisplayName = displayName;

            Session.Store(user);
            Session.SaveChanges();
        }

        private async Task<User> RegisterUser(string eMail, string displayName, string password)
        {
            var command = new RegisterUserCommand(
                null,
                new Name(displayName),
                new EMail(eMail),
                new Password(password),
                new Password(password));

            var result = await _mediator.Send(command);
            result.Succeeded.ShouldBeTrue(result.ToString);

            return await _userManager.FindByEmailAsync(eMail);
        }

        private void UpdateClients()
        {
            var clients = Clients.Domain.Clients.Get(_configuration);
            foreach (var client in clients)
            {
                EnsureClient(client);
            }
        }

        private void EnsureClient(Client client)
        {
            var existing = Session.Query<Client>().FirstOrDefault(criteria => criteria.Key == client.Key);
            if (existing == null)
            {
                Session.Store(client);
            }
            else
            {
                UpdateClient(client, existing);
                Session.Store(existing);
            }
        }

        private static void UpdateClient(Client client, Client existing)
        {
            existing.Secret = client.Secret;
            existing.Name = client.Name;
            existing.ApplicationType = client.ApplicationType;
            existing.Active = client.Active;
            existing.AllowedOrigin = client.AllowedOrigin;
            existing.RedirectUri = client.RedirectUri;
        }
    }
}