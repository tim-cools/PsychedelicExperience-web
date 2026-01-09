using System;
using Npgsql;
using PsychedelicExperience.Common.Store;

namespace PsychedelicExperience.Common.Tests.Storage
{
    public class TestStoreDatabaseFactory : ITestStoreDatabaseFactory
    {
        private readonly IConnectionStringParser _connectionStringParser;

        public TestStoreDatabaseFactory(IConnectionStringParser connectionStringParser)
        {
            _connectionStringParser = connectionStringParser ?? throw new ArgumentNullException(nameof(connectionStringParser));
        }

        public void CreateCleanStoreDatabase()
        {
            CreateDatabase();
            InstallPlv8();
        }

        private void InstallPlv8()
        {
            using (var connection = CreateConnection())
            using (var command = connection.CreateCommand())
            {
                connection.Open();

                command.CommandText = "CREATE EXTENSION plv8; CREATE EXTENSION plls;";
                command.ExecuteNonQuery();
            }

            //Log.Information("plv8 installed");
        }

        private void CreateDatabase()
        {
            var script = CreateScript();

            using var connection = CreateAdminConnection();
            using var command = connection.CreateCommand();

            connection.Open();

            command.CommandText = script;
            command.ExecuteNonQuery();

            //Log.Information("Test Store Database Created");
        }

        private NpgsqlConnection CreateAdminConnection()
        {
            var adminConnection = _connectionStringParser.GetString("documentStoreAdmin");
            //Log.Information($"Test Store Database Creating: {adminConnection}");
            return new NpgsqlConnection(adminConnection);
        }

        private NpgsqlConnection CreateConnection()
        {
            var connection = _connectionStringParser.GetString();
            //Log.Information($"Database Connection: {connection}");
            return new NpgsqlConnection(connection);
        }

        private string CreateScript()
        {
            var connectionString = _connectionStringParser.Parse();
            var script = typeof(TestStoreDatabaseFactory)
                .ReadResourceString("CreateStore.sql")
                .Replace("{database}", connectionString.Database)
                .Replace("{userId}", connectionString.UserId)
                .Replace("{password}", connectionString.Password);

            //Log.Information($"Script: {script}");
            return script;
        }
    }

}
