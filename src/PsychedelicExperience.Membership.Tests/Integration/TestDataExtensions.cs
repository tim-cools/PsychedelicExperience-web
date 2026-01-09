using System;
using System.Threading.Tasks;
using Baseline;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using Xunit.Abstractions;
using Role = PsychedelicExperience.Membership.Users.Domain.Role;

namespace PsychedelicExperience.Membership.Tests.Integration
{
    public static class TestDataExtensions
    {
        public static UserId AddUser(this TestContext<IMediator> context, params Role[] roles)
        {
            var id = new UserId(Guid.NewGuid());
            context.AddUser(id, roles);
            return id;
        }

        public static UserId AddAdministrator(this TestContext<IMediator> context)
        {
            var id = new UserId(Guid.NewGuid());
            context.AddAdministrator(id);
            return id;
        }

        public static TestContext<IMediator> AddAdministrator(this TestContext<IMediator> context, UserId id)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.AddUser(id);
            context.AddRoleToUser(id, Roles.Administrator).Wait();

            return context;
        }

        public static Users.Domain.User AddUser(this TestContext<IMediator> context, UserId id, params Role[] roles)
        {
            return AddUserAsync(context, id, roles).GetResult();
        }

        public static async Task<Users.Domain.User> AddUserAsync(this TestContext<IMediator> context, UserId id, params Role[] roles)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var manager = context.Container.GetInstance<UserManager<Users.Domain.User>>();
            var lookupNormalizer = context.Container.GetInstance<ILookupNormalizer>();

            var email = id + "@you.us";

            var user = new Users.Domain.User("me-" + id, email)
            {
                Id = (Guid)id,
                NormalizedEmail = lookupNormalizer.Normalize(email),
            };
            roles.Each(role => user.Roles.Add(role.Name));

            await manager.CreateAsync(user);
            await context.Session.SaveChangesAsync();

            return user;
        }

        public static async Task<TestContext<IMediator>> AddRoleToUser(this TestContext<IMediator> context, UserId id, string role)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var manager = context.Container.GetInstance<UserManager<Users.Domain.User>>();

            var user = await manager.FindByIdAsync(id.ToString());
            await manager.AddToRoleAsync(user, role);
            await context.Session.SaveChangesAsync();

            return context;
        }
    }
}
