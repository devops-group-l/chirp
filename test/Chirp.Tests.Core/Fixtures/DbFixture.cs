using Testcontainers.PostgreSql;

namespace Chirp.Tests.Core.Fixtures;

public class DbFixture : IDisposable
{
    private readonly PostgreSqlContainer _container;

    protected DbFixture()
    {
        _container = new PostgreSqlBuilder().Build();
        _container.StartAsync().Wait();

        Environment.SetEnvironmentVariable(
            "DB_STRING",
            _container.GetConnectionString()
        );
    }

    public void Dispose()
    {
        _container.DisposeAsync().AsTask().Wait();
    }
}
