using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;

namespace PsychedelicExperience.Common
{
    public static class ConfigurationExtensions
    {
        public static string Environment(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["environment:name"];
        }

        public static bool IsProduction(this IConfiguration configuration)
        {
            return configuration.Environment().Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDevelopment(this IConfiguration configuration)
        {
            return configuration.Environment().Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        public static bool SendMailEnabled(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["environment:sendMailEnabled"] == "true";
        }

        public static string LoggingFolder(this IConfiguration configuration)
        {
           if (configuration == null) throw new ArgumentNullException(nameof(configuration));
           return configuration["environment:logFolder"];
        }

        public static bool RunProjections(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            var runProjections = configuration["environment:runProjections"];
            return string.IsNullOrWhiteSpace(runProjections) || bool.Parse(runProjections);
        }

        public static string WebHostName(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["web:hostName"];
        }

        public static string WebUrl(this IConfiguration configuration, string url)
        {
            var hostName = configuration.WebHostName();

            if (hostName.EndsWith("/"))
            {
                return hostName + url.TrimStart('/');
            }
            if (!url.StartsWith("/"))
            {
                return hostName + "/" + url;
            }
            return hostName + url;
        }

        public static string WebUrl(this IConfiguration configuration, params object[] arguments)
        {
            var url = string.Join('/', arguments.Select(argument => argument?.ToString()).ToArray());
            return configuration.WebUrl(url);
        }

        public static string ApiHostName(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["api:hostName"];
        }

        public static string ApiTestAccountEMail(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["api:testAccount:email"];
        }

        public static string ApiTestAccountPassword(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["api:testAccount:password"];
        }

        public static string ApiClientId(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["api:clientId"];
        }

        public static string ApiClientSecret(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["api:clientSecret"];
        }

        public static bool GeneralConfigured(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return bool.TryParse(configuration["general:configured"], out var value) && value;
        }

        public static bool AuthenticationGoogleConfigured(this IConfiguration configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration.AuthenticationGoogleClientId())
                && !string.IsNullOrWhiteSpace(configuration.AuthenticationFacebookAppSecret());
        }

        public static string AuthenticationGoogleClientId(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["authentication:google:clientId"];
        }

        public static string AuthenticationGoogleClientSecret(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["authentication:google:clientSecret"];
        }

        public static bool AuthenticationFacebookConfigured(this IConfiguration configuration)
        {
            return !string.IsNullOrWhiteSpace(configuration.AuthenticationFacebookAppId())
               && !string.IsNullOrWhiteSpace(configuration.AuthenticationFacebookAppSecret());
        }

        public static string AuthenticationFacebookAppId(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["authentication:facebook:appId"];
        }

        public static string AuthenticationFacebookAppSecret(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["authentication:facebook:appSecret"];
        }

        public static string ConnectionString(this IConfiguration configuration, string name = "documentStore")
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            return configuration[$"connectionStrings:{name}"];
        }

        public static string SwaggerPath(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return configuration["swagger:xmlFileName"];
        }

        public static string PhotosFolder(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? configuration["photos:folder-win"]
                : configuration["photos:folder"];
        }

        public static string MailOptionsSmtpServer(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["mailOptions:SmtpServer"];
        }

        public static int MailOptionsSmtpPort(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return int.Parse(configuration["mailOptions:SmtpPort"]);
        }

        public static string MailOptionsSmtpUser(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["mailOptions:SmtpUser"];
        }

        public static string MailOptionsSmtpPassword(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["mailOptions:SmtpPassword"];
        }

        public static string MailOptionsFromEmailAddress(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["mailOptions:FromEmailAddress"];
        }

        public static string MailOptionsAdminEmailAddress(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["mailOptions:AdminEmailAddress"];
        }

        public static string DiscordToken(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["discord:token"];
        }

        public static ulong DiscordChannel(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return ulong.Parse(configuration["discord:channel"]);
        }

        public static string SendInBlueApi(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["sendInBlue:api"];
        }

       public static bool SendInBlueApiEnabled(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return !string.IsNullOrWhiteSpace(configuration["sendInBlue:api"]);
        }

        public static long SendInBlueNewsletterListId(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return long.Parse(configuration["sendInBlue:newsletterListId"]);
        }

        public static string HubspotApi(this IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return configuration["hubspot:api"];
        }
    }
}