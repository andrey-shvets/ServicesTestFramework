using System.Data.Common;
using System.Text.RegularExpressions;
using EvolveDb;
using MySqlConnector;

namespace ServicesTestFramework.DatabaseContainers;

public static class DatabaseMigrationExtensions
{
    public static List<string> ApplyMigrations(this DbConnection mySqlConnection, Dictionary<string, string> placeholders, params string[] migrationLocations)
    {
        var connectionString = GetConnectionString(mySqlConnection);
        var databaseName = GetDatabaseName(connectionString);

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        using var createDatabase = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {databaseName}", connection);
        createDatabase.ExecuteNonQuery();

        var evolve = new Evolve(connection) { MetadataTableName = "__changelog", Locations = migrationLocations, IsEraseDisabled = true, Placeholders = placeholders };

        evolve.Migrate();

        return evolve.AppliedMigrations;
    }

    private static string GetConnectionString(DbConnection dbConnection)
    {
        if (dbConnection is null)
            throw new ArgumentNullException(nameof(dbConnection), "Database connection can not be null.");

        var connectionString = dbConnection.ConnectionString;

        return connectionString;
    }

    private static string GetDatabaseName(string connectionString)
    {
        var databaseMatch = Regex.Match(connectionString, "(?<=Database=).+?(?=;)");

        if (!databaseMatch.Success)
            throw new ArgumentException($"Connection string does not contain database name. Connection string: {connectionString}", nameof(connectionString));

        var databaseName = databaseMatch.Value;

        return databaseName;
    }
}
