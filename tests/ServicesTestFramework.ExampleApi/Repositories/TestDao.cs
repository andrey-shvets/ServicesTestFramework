using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using ServicesTestFramework.ExampleApi.Configuration.Options;
using ServicesTestFramework.ExampleApi.Repositories.Entities;
using ServicesTestFramework.ExampleApi.Repositories.Interfaces;

namespace ServicesTestFramework.ExampleApi.Repositories;

public class TestDao : ITestDao
{
    private string ConnectionString { get; }

    public TestDao(IOptions<DatabaseOptions> databaseOptions)
    {
        ConnectionString = databaseOptions.Value.ConnectionString;
    }

    public async Task<IEnumerable<FirstEntity>> GetFirst()
    {
        await using var connection = new MySqlConnection(ConnectionString);
        var sql = "SELECT * FROM firstTable;";
        return await connection.QueryAsync<FirstEntity>(sql);
    }

    public async Task<IEnumerable<SecondEntity>> GetSecond()
    {
        await using var connection = new MySqlConnection(ConnectionString);
        var sql = "SELECT * FROM secondTable;";
        return await connection.QueryAsync<SecondEntity>(sql);
    }

    public async Task<IEnumerable<ThirdEntity>> GetThird()
    {
        await using var connection = new MySqlConnection(ConnectionString);
        var sql = "SELECT * FROM thirdTable;";
        return await connection.QueryAsync<ThirdEntity>(sql);
    }

    public async Task<IEnumerable<HotfixEntity>> GetHotfixTable()
    {
        await using var connection = new MySqlConnection(ConnectionString);
        var sql = "SELECT * FROM hotfixTable;";
        return await connection.QueryAsync<HotfixEntity>(sql);
    }
}
