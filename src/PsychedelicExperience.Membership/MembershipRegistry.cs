using System;
using System.Collections.Generic;
using AspNet.Security.OpenIdConnect.Primitives;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.StructureMap;
using PsychedelicExperience.Membership.Identity;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserContactLog;
using PsychedelicExperience.Membership.UserInfo;
using StructureMap;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership
{
    public class MembershipRegistry : Registry
    {
        public static IList<Type> CustomTypeConverters { get; } = JsonConverterResolver.Resolve<MembershipRegistry, UserId>();

        public MembershipRegistry()
        {
            Scan(options =>
            {
                options.TheCallingAssembly();
                options.Include(type => type.Name.EndsWith("Logger"));
                options.Include(type => type.Name.EndsWith("Projection"));
                options.Include(type => type.Name.EndsWith("Validator"));
                options.Include(type => type.Name.EndsWith("Handler"));
                options.Include(type => type.Name.EndsWith("Mapper"));

                options.IncludeNamespaceContainingType<UserStore>();

                options.IncludeNamespaceContainingType<IUserContactLogger>();
                options.IncludeNamespaceContainingType<IUserDataProtector>();
                options.IncludeNamespaceContainingType<IUserInfoResolver>();

                options.Convention<AllInterfacesConvention>();
            });

            For<IUserStore<User>>().Use<UserStore>();
            For<MartenRegistry>().Use<MembershipDocumentRegistry>();

            ForSingletonOf<IConfigureOptions<IdentityOptions>>()
                .Use("Create IdentityOptions",
                    context => new ConfigureOptions<IdentityOptions>(options =>
                    {
                        options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                        options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                        options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
                        options.User.RequireUniqueEmail = true;
                    }));

            foreach (var customTypeConverter in CustomTypeConverters)
            {
                For(typeof(CustomTypeConverter)).Use(customTypeConverter);
            }
        }
    }
}