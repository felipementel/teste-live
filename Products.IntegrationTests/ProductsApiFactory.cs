using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;

namespace Products.IntegrationTests;

public sealed class ProductsApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public bool IsDockerAvailable { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Database:Provider"] = "InMemory"
            };

            if (_container is not null)
            {
                settings["Database:Provider"] = "Postgres";
                settings["ConnectionStrings:Products"] = _container.GetConnectionString();
            }

            configurationBuilder.AddInMemoryCollection(settings);
        });
    }

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:16.4")
                .WithDatabase("products")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await _container.StartAsync();
            IsDockerAvailable = true;
        }
        catch (DockerUnavailableException)
        {
            IsDockerAvailable = false;
        }
    }

    public new async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }
}
