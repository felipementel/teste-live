using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Products.Application.Products;

namespace Products.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var products = app.MapGroup("/products");

        products.MapGet("/", async (IProductService service, CancellationToken cancellationToken) =>
            TypedResults.Ok(await service.GetAllAsync(cancellationToken)));

        products.MapGet("/{id:guid}", async (Guid id, IProductService service, CancellationToken cancellationToken) =>
        {
            var product = await service.GetByIdAsync(id, cancellationToken);
            return product is null ? Results.NotFound() : TypedResults.Ok(product);
        });

        products.MapPost("/", async (CreateProductRequest request, IProductService service, CancellationToken cancellationToken) =>
        {
            var created = await service.CreateAsync(request, cancellationToken);
            return Results.Created($"/products/{created.Id}", created);
        });

        products.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest request, IProductService service, CancellationToken cancellationToken) =>
        {
            var updated = await service.UpdateAsync(id, request, cancellationToken);
            return updated is null ? Results.NotFound() : TypedResults.Ok(updated);
        });

        products.MapDelete("/{id:guid}", async (Guid id, IProductService service, CancellationToken cancellationToken) =>
        {
            var deleted = await service.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
