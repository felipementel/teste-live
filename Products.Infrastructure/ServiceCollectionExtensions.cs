using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Products.Domain;

namespace Products.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Database:Provider"] ?? "InMemory";
        var connectionString = configuration.GetConnectionString("Products");

        switch (provider.ToLowerInvariant())
        {
            case "postgres":
                services.AddDbContext<ProductsDbContext>(options =>
                    options.UseNpgsql(connectionString));
                break;
            case "sqlserver":
                services.AddDbContext<ProductsDbContext>(options =>
                    options.UseSqlServer(connectionString));
                break;
            default:
                services.AddDbContext<ProductsDbContext>(options =>
                    options.UseInMemoryDatabase("products"));
                break;
        }

        services.AddScoped<IProductRepository, EfCoreProductRepository>();
        return services;
    }
}
