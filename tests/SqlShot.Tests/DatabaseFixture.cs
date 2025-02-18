using Npgsql;
using SqlShot.Tests;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(DatabaseFixture))]

namespace SqlShot.Tests;

public sealed class DatabaseFixture : IDisposable
{
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public NpgsqlDataSource DataSource { get; }

    public DatabaseFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder().Build();
        _postgreSqlContainer.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        DataSource = NpgsqlDataSource.Create(_postgreSqlContainer.GetConnectionString());
    }

    public void Dispose()
    {
        _postgreSqlContainer.StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }
}
