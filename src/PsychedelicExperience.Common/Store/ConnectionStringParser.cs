using System;
using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace PsychedelicExperience.Common.Store
{
    public class ConnectionStringParser : IConnectionStringParser
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringParser(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GetString(string name = "documentStore")
        {
            var connectionString = _configuration.ConnectionString(name);
            return string.IsNullOrWhiteSpace(connectionString) ? null : connectionString;
        }
        
        public ConnectionString Parse(string name = "documentStore")
        {
            var builder = new NpgsqlConnectionStringBuilder(GetString(name));

            return new ConnectionString(
                GetPart(builder, "Server"),
                GetPart(builder, "Port"),
                GetPart(builder, "database"),
                GetPart(builder, "User Id"),
                GetPart(builder, "password")
                );
        }

        private static string GetPart(DbConnectionStringBuilder builder, string name)
        {
            return builder[name]?.ToString();
        }
    }
}