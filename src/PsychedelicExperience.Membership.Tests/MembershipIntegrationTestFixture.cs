using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PsychedelicExperience.Common;
using StructureMap;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Users.Domain;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PsychedelicExperience.Membership.Tests
{
    public class MembershipIntegrationTestFixture : IntegrationTestFixture
    {
        protected override void InitializeContainer(ConfigurationExpression configuration)
        {
            configuration
                .ConfigureDummyIdentity()
                .AddRegistry<MembershipRegistry>();
        }
    }

    public static class IdentityContainerExtensions
    {
        public static ConfigurationExpression ConfigureDummyIdentity(this ConfigurationExpression configuration)
        {
            configuration.For<IUserValidator<User>>().Use<UserValidator<User>>();
            configuration.For<IPasswordValidator<User>>().Use<PasswordValidator<User>>();
            configuration.For<IPasswordHasher<User>>().Use<PasswordHasher<User>>();
            configuration.For<ILookupNormalizer>().Use<UpperInvariantLookupNormalizer>();
            configuration.For<IRoleValidator<Role>>().Use<RoleValidator<Role>>();
            configuration.For<ISecurityStampValidator>().Use<SecurityStampValidator<User>>();
            configuration.For<IUserClaimsPrincipalFactory<User>>().Use<UserClaimsPrincipalFactory<User, Role>>();
            configuration.For<UserManager<User>>().Use<UserManager<User>>();
            configuration.For<SignInManager<User>>().Use<SignInManager<User>>();
            configuration.For<RoleManager<Role>>().Use<RoleManager<Role>>();
            configuration.For<IOptions<IdentityOptions>>().Use<OptionsProvider>();
            configuration.For<IOptions<PasswordHasherOptions>>().Use<OptionsProvider>();
            configuration.For<IHttpContextAccessor>().Use<DummyHttpContextAccessor>();
            configuration.For<DummyUserTwoFactorTokenProvider>().Use<DummyUserTwoFactorTokenProvider>();

            return configuration;
        }
    }

    public class DummyLogger<T> : ILogger<T>, IDisposable
    {
        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        public IDisposable BeginScopeImpl(object state)
        {
            return this;
        }

        public void Dispose()
        {
        }
    }

    public class DummyHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }

    public class OptionsProvider : IOptions<IdentityOptions>, IOptions<PasswordHasherOptions>
    {
        private readonly IdentityOptions _options;

        IdentityOptions IOptions<IdentityOptions>.Value => _options;

        PasswordHasherOptions IOptions<PasswordHasherOptions>.Value => new PasswordHasherOptions();

        public OptionsProvider()
        {
            _options = new IdentityOptions();
            _options.Tokens.ProviderMap[TokenOptions.DefaultProvider] = new TokenProviderDescriptor(typeof(DummyUserTwoFactorTokenProvider));
            _options.Tokens.ProviderMap[TokenOptions.DefaultEmailProvider] = new TokenProviderDescriptor(typeof(DummyUserTwoFactorTokenProvider));
            _options.Tokens.ProviderMap[TokenOptions.DefaultPhoneProvider] = new TokenProviderDescriptor(typeof(DummyUserTwoFactorTokenProvider));
            _options.User.RequireUniqueEmail = true;
        }
    }
    
    public class DummyUserTwoFactorTokenProvider : IUserTwoFactorTokenProvider<User>
    {
        public Task<string> GenerateAsync(string purpose, UserManager<User> manager, User user)
        {
            return Task.FromResult("token-" +  DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        public Task<bool> ValidateAsync(string purpose, string token, UserManager<User> manager, User user)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> manager, User user)
        {
            return Task.FromResult(false);
        }
    }
}